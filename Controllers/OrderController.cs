using AutoMapper;
using ICSharpCode.SharpZipLib.Zip;
using Invitee.Entity;
using Invitee.Firebase;
using Invitee.Repository;
using Invitee.Utils;
using Invitee.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Invitee.Controllers
{
    [System.Web.Mvc.Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IRepositoryWrapper repositoryWrapper;
        private readonly IFireBaseAdmin fireBaseAdmin;
        private readonly ILogger logger;
        public OrderController(IRepositoryWrapper repositoryWrapper, IFireBaseAdmin fireBaseAdmin, ILogger logger)
        {
            this.repositoryWrapper = repositoryWrapper;
            this.fireBaseAdmin = fireBaseAdmin;
            this.logger = logger;
        }
        public ActionResult Index()
        {
            try
            {
                var orderList = repositoryWrapper.Order.FindAll().Where(x => x.OrderStatus != Entity.Enums.OrderStatus.Initiated).OrderByDescending(x => x.Id).ToList();
                var result = (Mapper.Map<IEnumerable<OrderViewModel>>(orderList));
                return View(result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }

        public async Task<ActionResult> StartOrderWork(int id)
        {
            var orderDB = repositoryWrapper.Order.FindByCondition(x => x.Id == id).Include("User").SingleOrDefault();
            if (orderDB != null)
            {
                orderDB.OrderStatus = Entity.Enums.OrderStatus.InProcess;
                orderDB.ProgressStartedAt = DateTime.Now;
                repositoryWrapper.Order.Update(orderDB);
                repositoryWrapper.Save();
                await fireBaseAdmin.SendNotificationAsync($"Your order {id} has been started!!", new List<string> { orderDB.User.DeviceId });

                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            return HttpNotFound();
        }

        [HttpPost]
        public async Task<ActionResult> FinishOrder(DeliveryViewModel deliveryModel)
        {

            try
            {
                if (deliveryModel.DFile == null && string.IsNullOrEmpty(deliveryModel.DeliveryFileUrl))
                {
                    throw new Exception("Please add file or url to be delivered.");
                }
                var order = repositoryWrapper.Order.FindByCondition(x => x.Id == deliveryModel.OrderId).SingleOrDefault();
                if (order == null)
                    throw new Exception("Order not found !");
                if (order.OrderStatus != Entity.Enums.OrderStatus.InProcess)
                    throw new Exception("Invalid order status !");
                if (deliveryModel.DFile != null)
                {
                    deliveryModel.DFile.ValidateVideoFile();
                    var newFileName = deliveryModel.DFile.GetNewFileName();
                    var deliveryFilePath = Server.MapPath($"~/{AppSettings.DeliveryFilePath}");
                    if (!Directory.Exists(deliveryFilePath))
                        Directory.CreateDirectory(deliveryFilePath);

                    deliveryModel.DFile.SaveAs(Path.Combine(deliveryFilePath, newFileName));
                    deliveryModel.DeliveryFile = $"{AppSettings.DeliveryFilePath}{newFileName}";
                }

                if (deliveryModel.CFile != null)
                {
                    deliveryModel.CFile.ValidateVideoFile();
                    var newFileName = deliveryModel.CFile.GetNewFileName();
                    var deliveryFilePath = Server.MapPath($"~/{AppSettings.DeliveryFilePath}");
                    if (!Directory.Exists(deliveryFilePath))
                        Directory.CreateDirectory(deliveryFilePath);

                    deliveryModel.CFile.SaveAs(Path.Combine(deliveryFilePath, newFileName));
                    deliveryModel.ComplementaryFile = $"{AppSettings.DeliveryFilePath}{newFileName}";
                }
                if (deliveryModel.CThumbnailFile != null)
                {
                    deliveryModel.CThumbnailFile.ValidateImageFile();
                    var newFileName = deliveryModel.CThumbnailFile.GetNewFileName();
                    var cFilePath = Server.MapPath($"~/{AppSettings.ComplementaryThumnailFilePath}");
                    if (!Directory.Exists(cFilePath))
                        Directory.CreateDirectory(cFilePath);

                    deliveryModel.CThumbnailFile.SaveAs(Path.Combine(cFilePath, newFileName));
                    deliveryModel.ComplementaryThumbnailFile = $"{AppSettings.ComplementaryThumnailFilePath}{newFileName}";
                }
                if (deliveryModel.DThumbnailFile != null)
                {
                    deliveryModel.DThumbnailFile.ValidateImageFile();
                    var newFileName = deliveryModel.DThumbnailFile.GetNewFileName();
                    var dFilePath = Server.MapPath($"~/{AppSettings.DeliveryThumnailFilePath}");
                    if (!Directory.Exists(dFilePath))
                        Directory.CreateDirectory(dFilePath);

                    deliveryModel.DThumbnailFile.SaveAs(Path.Combine(dFilePath, newFileName));
                    deliveryModel.DeliveryThumbnailFile = $"{AppSettings.DeliveryThumnailFilePath}{newFileName}";
                }
                deliveryModel.Id = Guid.NewGuid();

                var result = repositoryWrapper.Delivery.Create(Mapper.Map<Delivery>(deliveryModel));
                order.OrderStatus = Entity.Enums.OrderStatus.Completed;
                repositoryWrapper.Order.Update(order);
                repositoryWrapper.Save();
                var user = repositoryWrapper.Order.FindByCondition(x => x.Id == deliveryModel.OrderId).Include("User").SingleOrDefault()?.User;
                if (user != null)
                {
                    var baseUri = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
                    var emailTemplate = Properties.Resources.DownloadOrderTemplate
                        .Replace("[FullName]", user.FirstName + " " + user.LastName)
                        .Replace("[ordernumber]", "#ORD" + result.OrderId)
                        .Replace("[thumbnailurl]", baseUri + "/" + result.DeliveryThumbnailFile)
                        .Replace("[downloadurl]", AppSettings.CompleteOrderUrl);
                    await Mailer.SendEmail(user.Email, $"Order #ORD{result.OrderId} completed !", emailTemplate);
                    await fireBaseAdmin.SendNotificationAsync($"Your order {result.OrderId} has been completed", new List<string> { user.DeviceId });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
            }

            if (ModelState.IsValid)
            {
                return Json(new { success = true, message = "Success" });
            }
            else
            {
                var errorList = (from item in ModelState
                                 where item.Value.Errors.Any()
                                 select item.Value.Errors[0].ErrorMessage).ToList();
                return Json(new { success = false, data = errorList });
            }
        }

        [AllowAnonymous]
        public ActionResult Download(Guid id)
        {
            var delivery = repositoryWrapper.Delivery.FindByCondition(x => x.Id == id).SingleOrDefault();

            if (delivery != null)
            {
                delivery.DownloadedCount++;
                repositoryWrapper.Delivery.Update(delivery);
                repositoryWrapper.Save();
                if (!string.IsNullOrEmpty(delivery.DeliveryFile))
                {
                    var file = Server.MapPath($"~/{delivery.DeliveryFile}");
                    return File(file, MimeMapping.GetMimeMapping(file));
                }
                else
                {
                    return Redirect(delivery.DeliveryFileUrl);
                }
            }
            return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DownloadFile(int id)
        {
            var delivery = repositoryWrapper.Delivery.FindByCondition(x => x.OrderId == id).FirstOrDefault();
            if (delivery != null)
            {
                return RedirectToAction("Download", new { id = delivery.Id });
            }
            return HttpNotFound();
        }

        [HttpGet]
        public ActionResult DownloadOrderContent(int id)
        {
            logger.Information("Downloading content called");
            var order = repositoryWrapper.Order.FindByCondition(x => x.Id == id).FirstOrDefault();
            if (order != null)
            {
                var orderImages = order.OrderImages;
                var Order_id = order.Id;
                var ZipTem = order.MediaTemplate.TemplateName + "-" + Order_id;
                var newDirName = Guid.NewGuid();
                var tempDir = Server.MapPath($"~/Temp/{newDirName}");
                if (!Directory.Exists(tempDir))
                    Directory.CreateDirectory(tempDir);
                if (orderImages != null && orderImages.Count > 0)
                {
                    var imgCtr = 0;
                    foreach (var item in orderImages)
                    {
                        if (System.IO.File.Exists(Server.MapPath($"~/{item.ImagePath}")))
                            //System.IO.File.Copy(Server.MapPath($"~/{item.ImagePath}"), Path.Combine(tempDir, Path.GetFileName(item.ImagePath)));
                            System.IO.File.Copy(Server.MapPath($"~/{item.ImagePath}"), Path.Combine(tempDir, $"{++imgCtr}_{Path.GetFileName(item.ImagePath)}"));
                    }
                }
                if (!string.IsNullOrEmpty(order.AudioFilePath))
                {
                    if (System.IO.File.Exists(Server.MapPath($"~/{order.AudioFilePath}")))
                        System.IO.File.Copy(Server.MapPath($"~/{order.AudioFilePath}"), Path.Combine(tempDir, Path.GetFileName(order.AudioFilePath)));
                }

                if (order.OrderSlideTexts != null && order.OrderSlideTexts.Count > 0)
                {
                    var csv = new StringBuilder();
                    csv.AppendLine("ExistingSlideText, NewSlideText");

                    foreach (var item in order.OrderSlideTexts)
                    {
                        var newLine = $"{string.Format("\"{0}\"",item.SlideText.Text.Replace("\"", "\"\""))},{string.Format("\"{0}\"", item.NewSlideText.Replace("\"", "\"\""))}";
                        csv.AppendLine(newLine);
                    }
                    System.IO.File.WriteAllText(Path.Combine(tempDir, ZipTem + ".csv"), csv.ToString());
                }

                if (Directory.GetFiles(tempDir).Length == 0)
                {
                    return Json(new { message = "No Files found for this order" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    try
                    {
                        var zipName = ZipTem + ".zip";
                        CompressDirectory(tempDir, Server.MapPath($"~/Temp/{zipName}"));
                        Directory.Delete(tempDir, true);
                        return File(Path.Combine(Server.MapPath($"~/Temp"), zipName), "application/zip", zipName);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, ex.Message);
                        throw;
                    }
                }
            }
            return HttpNotFound();
        }

        /// <summary>
        /// Method that compress all the files inside a folder (non-recursive) into a zip file.
        /// </summary>
        /// <param name="DirectoryPath"></param>
        /// <param name="OutputFilePath"></param>
        /// <param name="CompressionLevel"></param>
        private void CompressDirectory(string DirectoryPath, string OutputFilePath, int CompressionLevel = 9)
        {
            try
            {
                // Depending on the directory this could be very large and would require more attention
                // in a commercial package.
                string[] filenames = Directory.GetFiles(DirectoryPath);

                // 'using' statements guarantee the stream is closed properly which is a big source
                // of problems otherwise.  Its exception safe as well which is great.
                using (ZipOutputStream OutputStream = new ZipOutputStream(System.IO.File.Create(OutputFilePath)))
                {

                    // Define the compression level
                    // 0 - store only to 9 - means best compression
                    OutputStream.SetLevel(CompressionLevel);

                    byte[] buffer = new byte[4096];

                    foreach (string file in filenames)
                    {

                        // Using GetFileName makes the result compatible with XP
                        // as the resulting path is not absolute.
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));

                        // Setup the entry data as required.

                        // Crc and size are handled by the library for seakable streams
                        // so no need to do them here.

                        // Could also use the last write time or similar for the file.
                        entry.DateTime = DateTime.Now;
                        OutputStream.PutNextEntry(entry);

                        using (FileStream fs = System.IO.File.OpenRead(file))
                        {

                            // Using a fixed size buffer here makes no noticeable difference for output
                            // but keeps a lid on memory usage.
                            int sourceBytes;

                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                OutputStream.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }

                    // Finish/Close arent needed strictly as the using statement does this automatically

                    // Finish is important to ensure trailing information for a Zip file is appended.  Without this
                    // the created file would be invalid.
                    OutputStream.Finish();

                    // Close is important to wrap things up and unlock the file.
                    OutputStream.Close();

                    Debug.WriteLine("Files successfully compressed");
                }
            }
            catch (Exception ex)
            {
                // No need to rethrow the exception as for our purposes its handled.
                Console.WriteLine("Exception during processing {0}", ex);
                throw ex;
            }
        }
        [HttpPost]
        public async Task<ActionResult> RejectOrder(int id, [System.Web.Http.FromBody] string reason)
        {
           
            var orderReject = repositoryWrapper.Order.FindByCondition(x => x.Id == id).Include("User").SingleOrDefault();
            if (orderReject != null)
            {
                try
                {
                    orderReject.OrderStatus = Entity.Enums.OrderStatus.Rejected;
                    orderReject.Reason = reason;
                    repositoryWrapper.Order.Update(orderReject);
                    repositoryWrapper.Save();
                    await fireBaseAdmin.SendNotificationAsync($"Your order {id} has been rejected!!\nReason : {reason}", new List<string> { orderReject.User.DeviceId });
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, error = ex.Message});
                }
            }
            return Json(new { success = false, error = "Order not found"});
        }
    }
}