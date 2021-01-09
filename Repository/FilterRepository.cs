using Invitee.Repository.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Invitee.Entity;

namespace Invitee.Repository
{
    public class FilterRepository : RepositoryBase<MediaFilter>, IFilterRepository
    {
        private RepositoryContext repositoryContext;
        public FilterRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            this.repositoryContext = repositoryContext;
        }
    }
}