using System.Net.Mail;
using System.Net;
using DoAn.Data;

namespace DoAn.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendBookingConfirmationAsync(string toEmail, string customerName, Booking booking)
        {
            string rows = "";
            foreach (var detail in booking.BookingDetails)
            {
                var days = (detail.CheckoutDate - detail.CheckinDate).Days;
                var roomTotal = detail.Price * days;
                var serviceTotal = booking.ServiceDetails
                    .Where(s => s.RoomId == detail.RoomId)
                    .Sum(s => s.Price * s.Amount);
                var total = roomTotal + serviceTotal;
                rows += $@"
                    <tr>
                        <td>{detail.Room.Room_No}</td>
                        <td>{detail.CheckinDate:yyyy-MM-dd}</td>
                        <td>{detail.CheckoutDate:yyyy-MM-dd}</td>
                        <td>{detail.Price} VNĐ</td>
                        <td>{days}</td>
                        <td>{roomTotal} VNĐ</td>
                        <td>{serviceTotal} VNĐ</td>
                        <td><strong>{total} VNĐ</strong></td>
                    </tr>";
            }
            string htmlBody = File.ReadAllText("Templates/BookingConfirmation.html")
                .Replace("{{CustomerName}}", customerName)
                .Replace("{{BookingId}}", booking.Id.ToString())
                .Replace("{{BookingDate}}", booking.BookingDate.ToString("yyyy-MM-dd HH:mm"))
                .Replace("{{PaymentMethod}}", booking.PaymentMethod)
                .Replace("{{Status}}", booking.status.ToString())
                .Replace("{{Note}}", booking.Note)
                .Replace("{{BookingLink}}", $"http://localhost:4200/#/my-bookings")
                .Replace("{{GrandTotal}}",  booking.TotalPrice.ToString("0.00"))
                .Replace("{{RoomName}}", "") 
                .Replace("{{CheckinDate}}", "")
                .Replace("{{CheckoutDate}}", "")
                .Replace("{{Price}}", "")
                .Replace("{{Unit}}", "")
                .Replace("{{Total}}", "")
                .Replace("</tbody>", rows + "</tbody>");


            var smtpClient = new SmtpClient(_configuration["Email:SmtpServer"])
            {
                Port = int.Parse(_configuration["Email:Port"]),
                Credentials = new NetworkCredential(_configuration["Email:Username"], _configuration["Email:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Email:From"]),
                Subject = "Đặt phòng thành công",
                Body = htmlBody,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation($"Email sent to {toEmail} for booking {booking.Id}");
        }

        public async Task SendEmailPwdDefault(string toEmail, string subject, string username)
        {
            string htmlTemplate = await File.ReadAllTextAsync("Templates/ResetEmail.html");
            string htmlBody = htmlTemplate.Replace("{{newPassword}}", username);
            var smtpClient = new SmtpClient(_configuration["Email:SmtpServer"])
            {
                Port = int.Parse(_configuration["Email:Port"]),
                Credentials = new NetworkCredential(_configuration["Email:Username"], _configuration["Email:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Email:From"]),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
        public async Task SendEmailAsync(string toEmail, string subject,string username)
        {
            string htmlTemplate = await File.ReadAllTextAsync("Templates/EmailTemplate.html");
            string htmlBody = htmlTemplate.Replace("{{username}}", username);
            var smtpClient = new SmtpClient(_configuration["Email:SmtpServer"])
            {
                Port = int.Parse(_configuration["Email:Port"]),
                Credentials = new NetworkCredential(_configuration["Email:Username"], _configuration["Email:Password"]),
                EnableSsl = true,
            };
            
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Email:From"]),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
