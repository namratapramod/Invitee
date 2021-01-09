using Invitee.Infrastructure;
using Invitee.Repository;
using Serilog;
using Invitee.Repository.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Invitee.ViewModels;
using System.Web.Http;
using AutoMapper;
using Invitee.Entity;
using Invitee.Utils;
using Org.BouncyCastle.Ocsp;
using System.IO;
using System.Data.Entity;
using Invitee.Firebase;
using System.Dynamic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.Linq.Dynamic;

namespace Invitee.ApiControllers
{
    [ApiAuthorize]
    public class OrderApiController : BaseApiController
    {
        private readonly IRepositoryWrapper repositoryWrapper;
        private readonly ILogger logger;
        private readonly IFireBaseAdmin fireBaseAdmin;
        public OrderApiController(IRepositoryWrapper repositoryWrapper, ILogger logger, IFireBaseAdmin fireBaseAdmin)
        {
            this.repositoryWrapper = repositoryWrapper;
            this.logger = logger.ForContext(this.GetType());
            this.fireBaseAdmin = fireBaseAdmin;
        }
        [System.Web.Http.HttpPost]
        public IHttpActionResult CreateOrder(OrderViewModel orderViewModel)
        {
            logger.Information("Create Order Called {@orderViewModel}", orderViewModel);
            if (ModelState.IsValid)
            {
                var mediaTempIsDelete = repositoryWrapper.MediaTemplate.FindByCondition(x => x.Id == orderViewModel.MediaTemplateId && x.IsDeleted == false).SingleOrDefault();

                // Validations
                if (mediaTempIsDelete == null)
                    return Error("Mediatemplate does not exist or has been deleted");
                if (orderViewModel.CostingId.HasValue)
                {
                    if (mediaTempIsDelete.Costings.Count > 0 && !mediaTempIsDelete.Costings.Where(x => x.Id == orderViewModel.CostingId.Value).Any())
                        return Error("The selected costing is not linked to the template please verify the costing details.");
                }
                if (orderViewModel.OrderSlideTexts?.Count > 0)
                {
                    var orderSlideIds = orderViewModel.OrderSlideTexts.Select(x => x.SlideTextId).ToArray();
                    var slideTextCount = mediaTempIsDelete.SlideTexts.Where(x => orderSlideIds.Contains(x.Id)).Count();
                    if (orderSlideIds.Length != slideTextCount)
                        return Error("One or more slide text does not belongs to the selecetd template.");
                }

                orderViewModel.OrderStatus = Entity.Enums.OrderStatus.Initiated;
                orderViewModel.UserId = GetUserId();


                if (mediaTempIsDelete.IsFree)
                {
                    var userInfo = repositoryWrapper.User.FindAll(true).Where(x => x.Id == orderViewModel.UserId).Single();
                    if (userInfo.EarnedVideo > 0)
                    {
                        userInfo.EarnedVideo -= 1;
                        userInfo.ClaimedVideo += 1;
                        orderViewModel.IsFreeOrder = true;
                    }
                }

                Order order = Mapper.Map<Order>(orderViewModel);
                var mediaFilters = repositoryWrapper.MediaFilter.FindByCondition(x => orderViewModel.MediaFilterIds.Contains(x.Id));
                if(orderViewModel.MediaFilterIds != null && orderViewModel.MediaFilterIds.Count != 0)
                {
                   order.MediaFilters = mediaFilters.ToList();
                }
                var result = repositoryWrapper.Order.Create(order);
                repositoryWrapper.Save();
                return Success(new { result.Id, result.OrderImages, result.UserId, result.OrderStatus, OrderSlideTexts = result.OrderSlideTexts.Select(x => new { x.NewSlideText, x.SlideTextId }) });
            }
            else
            {
                return ModelError();
            }
        }

        [System.Web.Http.HttpGet]
        public async Task<IHttpActionResult> CompleteOrder(int id)
        {
            logger.Information("CompleteOrder Called for order -" + id);
            var order = repositoryWrapper.Order.FindAll(true).Where(x => x.Id == id).SingleOrDefault();
            if (order != null)
            {
                //Validate the order 
                if (order.OrderStatus != Entity.Enums.OrderStatus.Initiated)
                    return Error("Order status is invalid!");
                if (order.MediaTemplate.IsAlbumTemplate && !order.OrderImages.Any())
                    return Error("No image has been uploaded for this album template order.");
                if (order.UserId != GetUserId())
                    return NotFound();
                var imageCosting = repositoryWrapper.ImageCost.FindAll()?.Select(x => new { x.ImageCount, x.Cost, x.Id, x.OfferInPercentage });
                var costingDet = new CostingDetails
                {
                    Costing = new
                    {
                        order.Costing?.Name,
                        CostingPrice = order.Costing?.CostToAdd,
                        order.Costing?.Description,
                        DeliveryHours = order.Costing?.DeliveryHours,
                        OfferPercentage = order.Costing?.OfferPercentage,
                        TemplateCost = order.MediaTemplate.NormalCost,
                        TemplateOffer = order.MediaTemplate.OfferPercentage,
                    },
                    GrandTotal = order.Costing?.CostToAdd - (order.Costing?.CostToAdd * ((Convert.ToDouble(order.Costing?.OfferPercentage)) / 100))
                };

                if (!order.IsFreeOrder)
                {
                    var normalCost = Convert.ToDouble(costingDet.Costing.GetType().GetProperty("TemplateCost").GetValue(costingDet.Costing));
                    var offer = Convert.ToDouble(costingDet.Costing.GetType().GetProperty("TemplateOffer").GetValue(costingDet.Costing));
                    if (costingDet.GrandTotal == null)
                        costingDet.GrandTotal = 0;
                    costingDet.GrandTotal += (normalCost - (normalCost * (offer / 100)));
                }
                if (order.MediaTemplate.IsAlbumTemplate && imageCosting!=null && imageCosting.Any())
                {
                    costingDet.ImageCosting = imageCosting;
                    var imageCnt = order.OrderImages == null ? 0 : order.OrderImages.Count();
                    var imCost = imageCosting.FirstOrDefault(x => x.ImageCount == imageCnt);
                    if (imCost == null)
                    {
                        var imgCosting =  imageCosting.Select(x => new { difference = Math.Abs(x.ImageCount - imageCnt), cost = x.Cost, offer = x.OfferInPercentage }).OrderBy(s => s.difference).First();
                        costingDet.GrandTotal += (imgCosting.cost - (imgCosting.cost * (Convert.ToDouble(imgCosting.offer) / 100)));
                    }
                    else
                    {
                        costingDet.GrandTotal += (imCost.Cost  - (imCost.Cost * (Convert.ToDouble(imCost.OfferInPercentage) /100 )));
                    }
                }

                order.OrderCostingJson = JsonConvert.SerializeObject(costingDet);
                order.OrderStatus = Entity.Enums.OrderStatus.New;
                repositoryWrapper.Save();
                var deviceId = GetDeviceId(this.repositoryWrapper);
                await fireBaseAdmin.SendNotificationAsync($"An order has been placed with orderid - " + order.Id, new List<string> { deviceId });

                var user = repositoryWrapper.Order.FindByCondition(x => x.Id == order.Id).Include("User").SingleOrDefault()?.User;
                var OrderCost = repositoryWrapper.CashfreePayment.FindAll(true).Where(x => x.OrderId == order.Id).SingleOrDefault();
                double orderAmount = 0;
                if(OrderCost != null)
                {
                    orderAmount = OrderCost.OrderAmount;
                }
                
                if (user != null)
                {
                    var OrderPlacedTemp = Properties.Resources.OrderPlaceEmail
                    .Replace("[FullName]", user.FirstName + " " + user.LastName)
                    .Replace("[ordernumber]", "#ORD" + order.Id)
                    .Replace("[TemplateName]", order.MediaTemplate.TemplateName)
                    .Replace("[OrderAmount]", orderAmount.ToString());
                    await Mailer.SendEmail(user.Email, $"Order #ORD{order.Id} has been placed successfully !", OrderPlacedTemp);
                    await Mailer.SendEmail(AppSettings.FromMailId, $"Order #ORD{order.Id} has been placed successfully !", OrderPlacedTemp);
                }
                return Success(JsonConvert.DeserializeObject(order.OrderCostingJson));
            }
            return NotFound();
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult GetMyOrders(int? id = null)
        {
            logger.Information("GetMyOrders Called {username}", GetUserName());
            var userId = GetUserId();
            var order = repositoryWrapper.Order.FindByCondition(x => x.UserId == userId &&  x.OrderStatus != Entity.Enums.OrderStatus.Initiated);
            if (id.HasValue && id.Value > 0)
            {
                order = order.Where(x => x.Id == id.Value);
                if (!order.Any())
                    return NotFound();
            }


            var result = order.Select(x => new OrderResponseModel
            {
                Id = x.Id,
                AudioFilePath = x.AudioFilePath,
                CategoryName = x.MediaTemplate.Category.Name,
                CategoryImage = x.MediaTemplate.Category.ImageUrl,
                TemplateName = x.MediaTemplate.TemplateName,
                IsAlbumTemplate = x.MediaTemplate.IsAlbumTemplate,
                TemplateDescription = x.MediaTemplate.TemplateDescription,
                TemplateId = x.MediaTemplate.Id,
                CreatedDate = x.CreatedDate,
                OrderImages = x.OrderImages.Select(im => new { im.ImagePath, im.ImageSize }),
                OrderSlideTextInfo = x.OrderSlideTexts.Select(os => new { os.SlideTextId, OriginalText = os.SlideText.Text, os.NewSlideText }),
                OrderStatus = x.OrderStatus,
                ProgressStartedAt = x.ProgressStartedAt,
                DeliveryInformation = x.Deliveries.Select(d => new { d.Id, d.DeliveryThumbnailFile, d.ComplementaryThumbnailFile, d.DeliveryFile, d.ComplementaryFile, d.ComplementaryFileUrl, d.DeliveryFileUrl, d.DownloadUrl, d.UpdatedDate, d.Rating, d.Comment, IsPublic = d.IsPublic.ToString().ToLower() }),
                Filters = x.MediaFilters.Select(mf => new { mf.Id, mf.Name, mf.Description }),
                Note = x.Note,
                IsFreeOrder = x.IsFreeOrder,
                OrderCostingJson = x.OrderCostingJson
            }).OrderByDescending(x => x.Id).ToList();

            var imageCosting = repositoryWrapper.ImageCost.FindAll().Select(x => new { x.ImageCount, x.Cost, x.Id, x.OfferInPercentage });
            foreach (var item in result)
            {
                if(!string.IsNullOrEmpty(item.OrderCostingJson))
                    item.CostingDetails = JsonConvert.DeserializeObject<CostingDetails>(item.OrderCostingJson);
            }
            return Success(result);
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult UploadImagesForOrder(int id)
        {
            logger.Information("UploadImagesForOrder Called for orderId: " + id);
            try
            {
                var order = repositoryWrapper.Order.FindByCondition(x => x.Id == id).SingleOrDefault();
                if (order == null)
                    return NotFound();


                if (order.OrderStatus != Entity.Enums.OrderStatus.Initiated)
                    return Error("Order status is invalid!");
                if (order.UserId != GetUserId())
                    return NotFound();

                var request = HttpContext.Current.Request;
                var httpPostedFile = new HttpPostedFileWrapper(request.Files[0]);
                httpPostedFile.ValidateImageFile();
                var newFileName = httpPostedFile.GetNewFileName();
                var orderImageDirectory = HttpContext.Current.Server.MapPath($"~/{AppSettings.OrderImagePath}{id}");
                if (!Directory.Exists(orderImageDirectory))
                    Directory.CreateDirectory(orderImageDirectory);
                httpPostedFile.SaveAs(Path.Combine(orderImageDirectory, newFileName));
                var orderImage = new OrderImage
                {
                    ImagePath = $"{AppSettings.OrderImagePath}{id}/{newFileName}",
                    OrderId = id,
                    ImageSize = httpPostedFile.ContentLength
                };
                repositoryWrapper.Order.AddOrderImage(orderImage);
                repositoryWrapper.Save();
                return Success(true);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                logger.Error(ex, "Error while uploading image for order " + id);
                return ModelError();
            }
        }

        public IHttpActionResult UploadAudioForOrder(int id)
        {
            logger.Information("UploadAudioForOrder Called for orderId: " + id);
            try
            {
                var order = repositoryWrapper.Order.FindByCondition(x => x.Id == id).SingleOrDefault();
                if (order == null)
                    return NotFound();
                if (order.OrderStatus != Entity.Enums.OrderStatus.Initiated)
                    return Error("Order status is invalid!");
                if (order.UserId != GetUserId())
                    return NotFound();

                var request = HttpContext.Current.Request;
                var httpPostedFile = new HttpPostedFileWrapper(request.Files[0]);
                httpPostedFile.ValidateAudioFile();
                var newFileName = httpPostedFile.GetNewFileName();
                var orderAudioDir = HttpContext.Current.Server.MapPath($"~/{AppSettings.OrderAudioPath}");
                if (!Directory.Exists(orderAudioDir))
                    Directory.CreateDirectory(orderAudioDir);
                httpPostedFile.SaveAs(Path.Combine(orderAudioDir, newFileName));
                order.AudioFilePath = $"{AppSettings.OrderAudioPath}/{newFileName}";
                repositoryWrapper.Order.Update(order);
                repositoryWrapper.Save();
                return Success(true);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                logger.Error(ex, "Error while uploading audio for order " + id);
                return ModelError();
            }
        }

        public IHttpActionResult MakePublic(Guid id)
        {
            logger.Information("MakePublic Called for Id: " + id);
            var userId = GetUserId();
            try
            {
                repositoryWrapper.Order.MakePublic(id, userId);
                repositoryWrapper.Save();
            }
            catch (InvalidOperationException ex)
            {

                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Forbidden, ex.Message));
            }
            return Success(true);
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult ListMyVideos()
        {
            logger.Information("ListMyVideos Called {username}", GetUserName());
            var userId = GetUserId();
            return Success(repositoryWrapper.Order.ListDeliveries(userId).Select(d => new
            {
                d.Id,
                d.OrderId,
                d.DeliveryThumbnailFile,
                d.DeliveryFile,
                d.DeliveryFileUrl,
                d.DownloadUrl,
                d.UpdatedDate,
                IsPublic = d.IsPublic,
                LikesCount = d.DeliveryLikes.Count,
                IsLiked = d.DeliveryLikes.Any(l => l.UserId == userId && l.DeliveryId == d.Id),
                UserInfo = new { d.Order.User.Id, d.Order.User.UserName, d.Order.User.ProfilePic, d.Order.User.Email },
                TemplateInfo = new { d.Order.MediaTemplate.TemplateName, d.Order.MediaTemplate.TemplateDescription },
                CategoryInfo = new { d.Order.MediaTemplate.Category.Name, d.Order.MediaTemplate.Category.Description, d.Order.MediaTemplate.Category.ImageUrl }
            }).OrderByDescending(d => d.OrderId).ToList());
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult ListPublicVideos()
        {
            logger.Information("ListPublicVideos Called {username}", GetUserName());
            var userId = GetUserId();
            return Success(repositoryWrapper.Order.ListDeliveries().Select(d => new
            {
                d.Id,
                d.OrderId,
                d.DeliveryThumbnailFile,
                d.DeliveryFile,
                d.DeliveryFileUrl,
                d.DownloadUrl,
                d.UpdatedDate,
                IsPublic = d.IsPublic,
                LikesCount = d.DeliveryLikes.Count,
                IsLiked = d.DeliveryLikes.Any(l => l.UserId == userId && l.DeliveryId == d.Id),
                UserInfo = new { d.Order.User.Id, d.Order.User.UserName, d.Order.User.ProfilePic, d.Order.User.Email },
                TemplateInfo = new { d.Order.MediaTemplate.TemplateName, d.Order.MediaTemplate.TemplateDescription },
                CategoryInfo = new { d.Order.MediaTemplate.Category.Name, d.Order.MediaTemplate.Category.Description, d.Order.MediaTemplate.Category.ImageUrl }
            }).OrderByDescending(d => d.OrderId).ToList());
        }

        public IHttpActionResult LikeUnlikeDelivery(Guid id)
        {
            logger.Information("LikeUnlikeDelivery Called {username}", GetUserName());
            var userId = GetUserId();
            this.repositoryWrapper.Order.LikeUnlike(id, userId);
            this.repositoryWrapper.Save();
            return Success(true);
        }

        public IHttpActionResult AddUpdateCommentAndRating(Guid id, [FromBody] Newtonsoft.Json.Linq.JObject jobject)
        {

            logger.Information("AddUpdateCommentAndRating Called {username}", GetUserName());
            try
            {
                var userId = GetUserId();
                if (!this.repositoryWrapper.Delivery.FindByCondition(x => x.Order.UserId == userId).Any())
                    throw new Exception("This order does not belongs to you!");
                var comment = jobject["comment"].ToString();
                var rating = Convert.ToInt32(jobject["rating"].ToString());
                this.repositoryWrapper.Order.AddUpdateCommentAndRating(comment, rating, id);
                this.repositoryWrapper.Save();
                return Success(true);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        void CalculateOrderCosting()
        {

        }
    }
}