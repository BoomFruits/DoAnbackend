using DoAn.Data;
using DoAn.DTO;
using DoAn.Helpers;
using DoAn.Service;
using Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPal.Api;
using System.Text.RegularExpressions;

namespace DoAn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IConfiguration _config;
        private readonly DbBookingContext _context;
        private readonly IVnPayService _vnPayService;
        private readonly IPaypalService _paypalService;

        public PaymentController(IConfiguration config, DbBookingContext context, IVnPayService vnPayService, IPaypalService paypalService)
        {
            _config = config;
            _context = context;
            _vnPayService = vnPayService;
            _paypalService = paypalService;
        }
        [HttpPost("vnpay/create-payment")]
        public IActionResult CreateVnPayPayment([FromBody] VnPaymentRequestModel model)
        {
            try
            {
                var paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, model);
                return Ok(new {paymentUrl});
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("paypal/create-payment")]
        public IActionResult CreatePaypalPayment([FromBody] PaypalCreatePaymentRequest request)
        {
            try
            {
            
                var payment = _paypalService.CreatePayment(request.BaseUrl,
                    request.Items,
                    request.Tax,
                    request.Shipping,
                    request.BookingId); // tax/shipping: 0 nếu không dùng

                return Ok(payment); // chứa danh sách links -> redirect Angular tới approval_url
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ✅ Handle return from VNPay
        [AllowAnonymous]
        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturn()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (!long.TryParse(response.OrderId.ToString(), out long bookingId))
                return BadRequest(new { success = false, message = "Invalid booking ID" });
            var booking = _context.Bookings.FirstOrDefault(b => b.Id == bookingId);
            if (booking == null)
                return NotFound();
            if (!response.Success)
                return Redirect($"http://localhost:4200/#/payment-failed?bookingId={bookingId}&method=VNPay");
            // Update trạng thái thanh toán
            booking.IsPaid = true;
            booking.status = 1;
            booking.PaymentMethod = "VNPay";
            booking.PaymentDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return Redirect($"http://localhost:4200/#/payment-success?bookingId={bookingId}&method=VNPay");
        }

        // ✅ Handle return from PayPal
        [AllowAnonymous]
        [HttpGet("paypal-return")]
        public async Task<IActionResult> PaypalSuccess(string paymentId, string payerId)
        {
            try
            {
                var payment = _paypalService.ExecutePayment(paymentId, payerId);

                // Lấy bookingId từ invoice_number
                var invoice = payment.transactions.FirstOrDefault()?.invoice_number;
                if (!long.TryParse(invoice, out long bookingId))
                    return BadRequest("Mã đơn đặt phòng không hợp lệ");

                var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
                if (booking == null)
                    return NotFound("Mã đơn đặt phòng không tìm thấy");

                booking.IsPaid = true;
                booking.status = 1;
                booking.PaymentMethod = "PayPal";
                booking.PaymentDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Redirect($"http://localhost:4200/#/payment-success?bookingId={bookingId}&method=PayPal");
            }
            catch (Exception ex)
            {
                var invoice = Request.Query["invoice"].ToString();
                return Redirect($"http://localhost:4200/#/payment-failed?bookingId={invoice}&method=PayPal");
            }
        }
    }
}
