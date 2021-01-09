﻿using Invitee.Entity;
using Invitee.Repository.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.Repository
{
    public class OfferBannerRepository : RepositoryBase<OfferBanner>, IOfferBannerRepository
    {
        private RepositoryContext repositoryContext;    
        public OfferBannerRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            this.repositoryContext = repositoryContext;
        }
    }
}