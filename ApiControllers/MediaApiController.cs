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
    public class MediaApiController : BaseApiController
    {
        private readonly IRepositoryWrapper repositoryWrapper;
        private readonly ILogger logger;
        public MediaApiController(IRepositoryWrapper repositoryWrapper, ILogger logger)
        {
            this.logger = logger.ForContext(this.GetType());
            this.repositoryWrapper = repositoryWrapper;
        }

        [HttpGet]
        public IHttpActionResult GetMediaTemplates(int? id = null, bool isFree = false, string templateType = "Album")
        {
            logger.Information("GetMediaTemplates called by {username}", GetUserName());
            if (templateType.ToLower() != "album" && templateType.ToLower() != "video")
                return Error("Invalid templateType");
            var userId = GetUserId();
            var isAlbumTemplate = templateType.ToLower() == "album";
            var result = this.repositoryWrapper.MediaTemplate.FindAll(true).Where(x => x.IsFree == isFree && x.IsAlbumTemplate == isAlbumTemplate && x.IsDeleted == false);
            if (id.HasValue)
                result = result.Where(x => x.CategoryId == id);

            var imageCosting = repositoryWrapper.ImageCost.FindAll().Select(im => new { im.Id, im.ImageCount, im.Cost, im.OfferInPercentage });
            var returnData = new
            {
                MediaTemplates = result.Select(x => new
                {
                    x.Id,
                    x.TemplateName,
                    x.TemplateDescription,
                    CategoryName = x.Category.Name,
                    x.IsAlbumTemplate,
                    x.IsFree,
                    x.NormalCost,
                    SlideTexts = x.SlideTexts.Select(s => new { s.Id, s.Text }),
                    x.VideoFilePath,
                    x.VideoThumbnail,
                    x.OfferPercentage,
                    Costings = x.Costings.Select(c => new { c.Id, c.CostToAdd, c.DeliveryHours, c.Name, c.Description, c.OfferPercentage }),
                    LikesCount = x.MediaTemplateLikes.Count,
                    IsLiked = x.MediaTemplateLikes.Any(l => l.UserId == userId && l.MediaTemplateId == x.Id),
                    x.ImageCount
                }),
                ImageCosting = imageCosting
            };

            return Success(returnData);
        }

        public IHttpActionResult LikeUnlikeMediaTemplate(int id)
        {
            var userId = GetUserId();
            this.repositoryWrapper.MediaTemplate.LikeUnlike(id, userId);
            this.repositoryWrapper.Save();
            return Success(true);
        }

        [HttpGet]
        public IHttpActionResult GetFilters()
        {
            return Success(repositoryWrapper.MediaFilter.FindAll().Select(x => new { x.Id, x.Name, x.Description, x.CreatedDate, x.UpdatedDate }));
        }

    }
}
