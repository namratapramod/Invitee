using Invitee.Entity;
using Invitee.Repository.Infra;

namespace Invitee.Repository
{
    public interface IPaymentRepository : IRepository<CashfreePayment>
    {
    }
}