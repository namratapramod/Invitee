using Invitee.Entity;
using Invitee.Repository.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.Repository
{
    public class CostingRepository : RepositoryBase<Costing>, ICostingRepository
    {
        private RepositoryContext repositoryContext;
        public CostingRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            this.repositoryContext = repositoryContext;
        }
    }
}