using Invitee.Models.Payment;
using System.Threading.Tasks;

namespace Invitee.Infrastructure.PaymentInfra
{
    public interface IPaymentService
    {
        Task<PaymentResponce> GetCftoken(PaymentRequest paymentRequest);
        bool UpdatePaymentStatus(int orderId, bool status);
    }
}