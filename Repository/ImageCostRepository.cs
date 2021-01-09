using Invitee.Entity;
using Invitee.Repository.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.Repository
{
    public class ImageCostRepository : RepositoryBase<ImageCost>, IImageCostRepository
    {
        private RepositoryContext repositoryContext;
        public ImageCostRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            this.repositoryContext = repositoryContext;
        }
    }
}