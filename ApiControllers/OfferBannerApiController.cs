using Invitee.Infrastructure;
using Invitee.Repository;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Invitee.ApiControllers
{
    [ApiAuthorize]
    public class OfferBannerApiController : BaseApiController
    {
        private readonly IRepositoryWrapper repositoryWrapper;
        private readonly ILogger logger;
        public OfferBannerApiController(IRepositoryWrapper repositoryWrapper, ILogger logger)
        {
            this.repositoryWrapper = repositoryWrapper;
            this.logger = logger.ForContext(this.GetType());
        }
        [HttpGet]
        public IHttpActionResult GetOfferBanner(string offerType = "text")
        {
            logger.Information("GetofferBanner called by {username}", GetUserName());
            if(offerType == "text")
            {
                var data = this.repositoryWrapper.OfferBanner.FindAll().Where(x => x.IsImage == false).OrderByDescending(x => x.Id).ToList();
                return Success(data);
            }
            var isImageType = offerType == "image";
            var result = this.repositoryWrapper.OfferBanner.FindAll().Where(x => x.IsImage == isImageType).ToList();
            return Success(result);
        }
    }
}