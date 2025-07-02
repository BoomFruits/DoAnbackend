
using DoAn.DTO;
using PayPal;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace DoAn.Service
{
    public class PaypalService : IPaypalService
    {
        private readonly APIContext _apiContext;
        private readonly PaypalConfiguration _paypalConfig;
        public PaypalService(PaypalConfiguration paypalConfiguration)
        {
            _apiContext = paypalConfiguration.GetAPIContext();
        }
        public APIContext GetAPIContext()
        {
            return _paypalConfig.GetAPIContext();
        }
        public Payment CreatePayment(string baseUrl, List<Item> items, double tax, double shipping,long bookingId)
        {
            try
            {
                var apiContext = _apiContext;
                var subtotal = items.Sum(i => Convert.ToDouble(i.price, CultureInfo.InvariantCulture) * Convert.ToInt32(i.quantity));
                var total = subtotal + tax + shipping; 
                var amount = new Amount
                {
                    currency = "USD",
                    total = (subtotal + tax + shipping).ToString("F2", CultureInfo.InvariantCulture),
                    details = new Details
                    {
                        tax = tax.ToString("F2", CultureInfo.InvariantCulture),
                        shipping = shipping.ToString("F2", CultureInfo.InvariantCulture),
                        subtotal = subtotal.ToString("F2", CultureInfo.InvariantCulture)
                    }
                };

                var transaction = new Transaction
                {
                    amount = amount,
                    item_list = new ItemList { items = items },
                    description = "Payment for your order",
                    invoice_number = bookingId.ToString()
                };

                var payment = new Payment
                {
                    intent = "sale",
                    payer = new Payer { payment_method = "paypal" },
                    transactions = new List<Transaction> { transaction },
                    redirect_urls = new RedirectUrls
                    {
                        return_url = "https://localhost:7275/api/payment/paypal-return",
                        cancel_url = "https://localhost:7275/api/payment/paypal-cancel"
                    }
                };

                // Gọi API PayPal để tạo payment
                return payment.Create(apiContext);
            }
            catch (PaymentsException ex)
            {
                var error = ex.Details;
                throw new Exception($"PayPal Error: {error.name} - {error.message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi không xác định khi tạo PayPal Payment", ex);
            }
        }

        /// Thực hiện thanh toán PayPal sau khi khách hàng xác nhận
        public Payment ExecutePayment(string paymentId, string payerId)
        {
            try
            {
                var paymentExecution = new PaymentExecution() { payer_id = payerId };
                var payment = new Payment() { id = paymentId };
                return payment.Execute(_apiContext, paymentExecution);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thực hiện Payment trên PayPal", ex);
            }
        }
    }
}
