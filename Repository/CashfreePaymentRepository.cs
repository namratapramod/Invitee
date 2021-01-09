using Invitee.Entity;
using Invitee.Repository.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.Repository
{
    public class CashfreePaymentRepository : RepositoryBase<CashfreePayment>, ICashfreePaymentRepository
    {
        private RepositoryContext repositoryContext;
        public CashfreePaymentRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            this.repositoryContext = repositoryContext;
        }
    }
}