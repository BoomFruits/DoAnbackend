
namespace DoAn.Service
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext httpContext,VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collection);
    }
}
