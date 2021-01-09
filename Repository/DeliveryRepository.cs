using Invitee.Entity;
using Invitee.Repository.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.Repository
{
    public class DeliveryRepository : RepositoryBase<Delivery>, IDeliveryRepository
    {
        private readonly RepositoryContext repositoryContext;
        public DeliveryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            this.repositoryContext = repositoryContext;
        }
    }
}