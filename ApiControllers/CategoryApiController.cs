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
    public class CategoryApiController : BaseApiController
    {
        private readonly IRepositoryWrapper repositoryWrapper;
        private readonly ILogger logger;
        public CategoryApiController(IRepositoryWrapper repositoryWrapper, ILogger logger)
        {
            this.repositoryWrapper = repositoryWrapper;
            this.logger = logger.ForContext(this.GetType());
        }

        public IHttpActionResult GetCategories()
        {
            logger.Information("GetCategories called by {username}", GetUserName());
            return Success(this.repositoryWrapper.Category.FindAll().Include("ChildCategories").Select(x=> new { 
                x.Id,
                Name = x.Name,
                Description = x.Description,
                Image = x.ImageUrl,
                SubCategoriesCount = x.ChildCategories.Count,
                x.ExtraInputOne,
                x.ExtraInputTwo,
                SubCategories = x.ChildCategories.Select(s=> new
                {
                    Name = s.Name,
                    Id = s.Id,
                    s.Description,
                    s.ImageUrl
                })
            }));
        }
    }
}
