using AutoMapper;
using Invitee.Entity;
using Invitee.Repository;
using Invitee.Utils;
using Invitee.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Invitee.Controllers
{
    [System.Web.Mvc.Authorize(Roles = "Admin")]
    public class MediaTemplateController : Controller
    {
        private readonly IRepositoryWrapper repositoryWrapper;

        public MediaTemplateController(IRepositoryWrapper repositoryWrapper)
        {
            this.repositoryWrapper = repositoryWrapper;
        }
        // GET: MediaTemplate
        public ActionResult Index()
        {
            try
            {
                var result = repositoryWrapper.MediaTemplate.FindByCondition(x=>x.IsDeleted == false).Include("SlideTexts").Include("Category").Include("Costings");
                ViewBag.categoryList = repositoryWrapper.Category.FindAll().OrderBy(s => s.Name).ToList();
                return View(Mapper.Map<IEnumerable<MediaTemplateViewModel>>(result));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }
        
        public ActionResult Create()
        {
            var templateViewModel = new MediaTemplateViewModel {
                CategoryList = repositoryWrapper.Category.FindAll().OrderBy(x => x.Name).ToList(),
                CostingList = repositoryWrapper.Costing.FindAll().OrderBy(x => x.Name).ToList()
            };
            if (!string.IsNullOrEmpty(Request.QueryString["category"]))
                templateViewModel.CategoryId = Convert.ToInt32(Request.QueryString["category"]);
            return View(templateViewModel);
        }

        [HttpPost]
        public ActionResult UpdateSlideTexts(int mediaTemplateId, string slideTexts)
        {
            this.repositoryWrapper.MediaTemplate.UpdateSlideTexts(mediaTemplateId, slideTexts.Split(new string[] { "$=$" },StringSplitOptions.RemoveEmptyEntries));
            return Json("Success");
        }

        [HttpPost]
        public ActionResult Create(MediaTemplateViewModel mediaTemplateViewModel)
        {
            mediaTemplateViewModel.CategoryList = repositoryWrapper.Category.FindAll().OrderBy(x => x.Name).ToList();
            mediaTemplateViewModel.CostingList = repositoryWrapper.Costing.FindAll().OrderBy(x => x.Name).ToList();
            if (mediaTemplateViewModel.IsAlbumTemplate)
                mediaTemplateViewModel.IsFree = false;
            if (ModelState.IsValid)
            {
                try
                {
                    if (mediaTemplateViewModel.VideoFile == null)
                        throw new Exception("Video file is required!");
                    if (mediaTemplateViewModel.VideoThumbnailFile == null)
                        throw new Exception("Video thumbnail file is required!");

                    mediaTemplateViewModel.VideoFile.ValidateVideoFile();
                    mediaTemplateViewModel.VideoThumbnailFile.ValidateImageFile();
                     
                    var newVideoFileName = $"{Guid.NewGuid()}{Path.GetExtension(mediaTemplateViewModel.VideoFile.FileName)}";
                    var newVideoThumbFileName = $"{Guid.NewGuid()}{Path.GetExtension(mediaTemplateViewModel.VideoThumbnailFile.FileName)}";

                    string videoServerPath = Server.MapPath($"~/{AppSettings.VideoFilePath}");
                    if (!Directory.Exists(videoServerPath))
                        Directory.CreateDirectory(videoServerPath);

                    string thumbnailServerPath = Server.MapPath($"~/{AppSettings.VideoThumnailFilePath}");
                    if (!Directory.Exists(thumbnailServerPath))
                        Directory.CreateDirectory(thumbnailServerPath);

                    mediaTemplateViewModel.VideoThumbnailFile.SaveAs(Path.Combine(thumbnailServerPath, newVideoThumbFileName));
                    mediaTemplateViewModel.VideoFile.SaveAs(Path.Combine(videoServerPath, newVideoFileName));

                    mediaTemplateViewModel.VideoFilePath = AppSettings.VideoFilePath + newVideoFileName;
                    mediaTemplateViewModel.VideoThumbnail = AppSettings.VideoThumnailFilePath + newVideoThumbFileName;
                    if(mediaTemplateViewModel.SlideTextInput != null)
                    {
                        var slideArray = mediaTemplateViewModel.SlideTextInput.Split('|');
                        mediaTemplateViewModel.SlideTexts = new List<SlideText>();
                        foreach (var item in slideArray)
                        {
                            mediaTemplateViewModel.SlideTexts.Add(new SlideText
                            {
                                Text = item
                            });
                        }
                    }

                    if(mediaTemplateViewModel.CostingIds!=null)
                        mediaTemplateViewModel.Costings = repositoryWrapper.Costing.FindAll(true).Where(x => mediaTemplateViewModel.CostingIds.Contains(x.Id)).ToList();

                    var mediaTemplate = Mapper.Map<MediaTemplate>(mediaTemplateViewModel);
                    repositoryWrapper.MediaTemplate.Create(mediaTemplate);
                    repositoryWrapper.Save();
                    ViewBag.Success = true;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", ex.Message);
                }
            }
            return View(mediaTemplateViewModel);
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult Edit(int? id)
        {
            var mediaTemp = repositoryWrapper.MediaTemplate.FindByCondition(x => x.Id == id);
            if (!mediaTemp.Any())
                return HttpNotFound();
            var mediaTempModel = Mapper.Map<MediaTemplateViewModel>(mediaTemp.Single());
            mediaTempModel.CategoryList = repositoryWrapper.Category.FindAll().OrderBy(x => x.Name).ToList();
            mediaTempModel.CostingList = repositoryWrapper.Costing.FindAll().OrderBy(x => x.Name).ToList();
            mediaTempModel.CostingIds = mediaTempModel.Costings.Select(x => x.Id).ToArray();
            mediaTempModel.SlideTextInput = string.Join(",", mediaTempModel.SlideTexts.Select(x => x.Text).ToArray());
            return View(mediaTempModel);
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult Edit(MediaTemplateViewModel mediaTemplateViewModel)
        {
            if (mediaTemplateViewModel.IsAlbumTemplate)
                mediaTemplateViewModel.IsFree = false;

            mediaTemplateViewModel.CategoryList = repositoryWrapper.Category.FindAll().OrderBy(x => x.Name).ToList();
            mediaTemplateViewModel.CostingList = repositoryWrapper.Costing.FindAll().OrderBy(x => x.Name).ToList();
            if (ModelState.IsValid)
            {
                try
                {
                    if(mediaTemplateViewModel.VideoFile != null)
                    {
                        //Delete existing file if exists 
                        if (!string.IsNullOrEmpty(mediaTemplateViewModel.VideoFilePath) && System.IO.File.Exists(Path.Combine(Server.MapPath($"~/{AppSettings.VideoFilePath}"), mediaTemplateViewModel.VideoFilePath)))
                        {
                            System.IO.File.Delete(Path.Combine(Server.MapPath($"~/{AppSettings.VideoFilePath}"), mediaTemplateViewModel.VideoFilePath));
                        }
                        mediaTemplateViewModel.VideoFile.ValidateVideoFile();

                        var newVideoFileName = $"{Guid.NewGuid()}{Path.GetExtension(mediaTemplateViewModel.VideoFile.FileName)}";
                        mediaTemplateViewModel.VideoFilePath = newVideoFileName;
                        string videoServerPath = Server.MapPath($"~/{AppSettings.VideoFilePath}");
                        if (!Directory.Exists(videoServerPath))
                            Directory.CreateDirectory(videoServerPath);

                        mediaTemplateViewModel.VideoFile.SaveAs(Path.Combine(videoServerPath, newVideoFileName));
                        mediaTemplateViewModel.VideoFilePath = AppSettings.VideoFilePath + newVideoFileName;
                    }
                    if (mediaTemplateViewModel.VideoThumbnailFile != null)
                    {
                        //Delete existing file if exists 
                        if (!string.IsNullOrEmpty(mediaTemplateViewModel.VideoThumbnail) && System.IO.File.Exists(Path.Combine(Server.MapPath($"~/{AppSettings.VideoThumnailFilePath}"), mediaTemplateViewModel.VideoThumbnailFile.FileName)))
                        {
                            System.IO.File.Delete(Path.Combine(Server.MapPath($"~/{AppSettings.VideoThumnailFilePath}"), mediaTemplateViewModel.VideoThumbnailFile.FileName));
                        }
                        mediaTemplateViewModel.VideoThumbnailFile.ValidateImageFile();
                        var newVideoThumbFileName = $"{Guid.NewGuid()}{Path.GetExtension(mediaTemplateViewModel.VideoThumbnailFile.FileName)}";

                        string thumbnailServerPath = Server.MapPath($"~/{AppSettings.VideoThumnailFilePath}");
                        if (!Directory.Exists(thumbnailServerPath))
                            Directory.CreateDirectory(thumbnailServerPath);

                        mediaTemplateViewModel.VideoThumbnailFile.SaveAs(Path.Combine(thumbnailServerPath, newVideoThumbFileName));
                        mediaTemplateViewModel.VideoThumbnail = AppSettings.VideoThumnailFilePath + newVideoThumbFileName;
                        
                    }
                   


                    if (mediaTemplateViewModel.CostingIds != null)
                        mediaTemplateViewModel.Costings = repositoryWrapper.Costing.FindByCondition(x => mediaTemplateViewModel.CostingIds.Contains(x.Id)).ToList();

                    var mediaTemplate = Mapper.Map<MediaTemplate>(mediaTemplateViewModel);
                    
                    repositoryWrapper.MediaTemplate.Update(mediaTemplate);
                    repositoryWrapper.Save();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", ex.Message);
                    return View(mediaTemplateViewModel);
                }
                return RedirectToAction("Index");
                //ViewBag.Success = true;
            }
          
            return View(mediaTemplateViewModel);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var mediaTemplateToDel = repositoryWrapper.MediaTemplate.FindByCondition(x => x.Id == id);
            if (mediaTemplateToDel.Any())
            {
                repositoryWrapper.MediaTemplate.Delete(mediaTemplateToDel.Single());
                repositoryWrapper.Save();
                return Json("Success");
            }
            Response.StatusCode = 404;
            return Json("Costing not found");
        }
    }
}