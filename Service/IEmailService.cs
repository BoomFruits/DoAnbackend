using DoAn.Data;

namespace DoAn.Service
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string username);
        Task SendBookingConfirmationAsync(string toEmail, string customerName, Booking booking);
        Task SendEmailPwdDefault(string toEmail, string subject, string username);
    }
}
