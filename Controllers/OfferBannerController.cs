using AutoMapper;
using Invitee.Entity;
using Invitee.Repository;
using Invitee.Utils;
using Invitee.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Invitee.Controllers
{
    [System.Web.Mvc.Authorize(Roles = "Admin")]
    public class OfferBannerController : Controller
    {

        private readonly IRepositoryWrapper repositoryWrapper;
        public OfferBannerController(IRepositoryWrapper repositoryWrapper)
        {
            this.repositoryWrapper = repositoryWrapper;
        }
        public ActionResult Index()
        {
            var result = repositoryWrapper.OfferBanner.FindAll().ToList();
            return View(Mapper.Map<IEnumerable<OfferBannerViewModel>>(result));
        }
        public ActionResult Create()
        {
            return View(new OfferBannerViewModel());
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult Create(OfferBannerViewModel offerBannerViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (offerBannerViewModel.File != null)
                    {
                        offerBannerViewModel.File.ValidateImageFile();
                        var newFileName = offerBannerViewModel.File.GetNewFileName();
                        offerBannerViewModel.ImageUrl = $"{AppSettings.OfferBannerImagesPath}{newFileName}";
                        if (!Directory.Exists(Server.MapPath($"~/{AppSettings.OfferBannerImagesPath}")))
                            Directory.CreateDirectory(Server.MapPath($"~/{AppSettings.OfferBannerImagesPath}"));
                        offerBannerViewModel.File.SaveAs(Path.Combine(Server.MapPath($"~/{AppSettings.OfferBannerImagesPath}"), newFileName));
                    }
                    if (offerBannerViewModel.IsImage == false)
                    {
                        if (!string.IsNullOrEmpty(offerBannerViewModel.ImageUrl) && !System.IO.File.Exists(Path.Combine(Server.MapPath($"~/{AppSettings.OfferBannerImagesPath}"), offerBannerViewModel.ImageUrl)))
                        {
                            offerBannerViewModel.ImageUrl = null;
                        }
                    }
                    else
                    {
                        offerBannerViewModel.OfferText = null;
                    }
                    /*else
                    {
                       throw new Exception("Offer banner file is required!");
                    }*/

                    var offerBanner = Mapper.Map<OfferBanner>(offerBannerViewModel);
                    repositoryWrapper.OfferBanner.Create(offerBanner);
                    repositoryWrapper.Save();
                    ViewBag.Success = true;

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", ex.Message);
                    return View(offerBannerViewModel);
                }
                return RedirectToAction("Index");
            }
            return View(offerBannerViewModel);
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult Edit(int? id)
        {
            var offerBanners = repositoryWrapper.OfferBanner.FindByCondition(x => x.Id == id);
            if (!offerBanners.Any())
                return HttpNotFound();
            var offerBannerViewModel = Mapper.Map<OfferBannerViewModel>(offerBanners.Single());
            return View(offerBannerViewModel);
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult Edit(OfferBannerViewModel offerBannerViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (offerBannerViewModel.File != null)
                    {
                        //Delete existing file if exists 
                        if (!string.IsNullOrEmpty(offerBannerViewModel.ImageUrl) && System.IO.File.Exists(Path.Combine(Server.MapPath("~/Images/CategoryImages"), offerBannerViewModel.ImageUrl)))
                        {
                            System.IO.File.Delete(Path.Combine(Server.MapPath($"~/{AppSettings.OfferBannerImagesPath}"), offerBannerViewModel.ImageUrl));
                        }
                        offerBannerViewModel.File.ValidateImageFile();
                        var newFileName = $"{Guid.NewGuid()}{Path.GetExtension(offerBannerViewModel.File.FileName)}";
                        offerBannerViewModel.ImageUrl = $"{AppSettings.OfferBannerImagesPath}{newFileName}";
                       // offerBannerViewModel.ImageUrl = newFileName;
                        if (!Directory.Exists(Server.MapPath($"~/{AppSettings.OfferBannerImagesPath}")))
                            Directory.CreateDirectory(Server.MapPath($"~/{AppSettings.OfferBannerImagesPath}"));
                        offerBannerViewModel.File.SaveAs(Path.Combine(Server.MapPath($"~/{AppSettings.OfferBannerImagesPath}"), newFileName));
                    }
                    if (offerBannerViewModel.IsImage == false)
                    {
                        if (!string.IsNullOrEmpty(offerBannerViewModel.ImageUrl) && !System.IO.File.Exists(Path.Combine(Server.MapPath($"~/{AppSettings.OfferBannerImagesPath}"), offerBannerViewModel.ImageUrl)))
                        {
                            offerBannerViewModel.ImageUrl = null;
                        }
                    }
                    else
                    {
                        offerBannerViewModel.OfferText = null;
                    }

                        var offerBanner = Mapper.Map<OfferBanner>(offerBannerViewModel);
                    repositoryWrapper.OfferBanner.Update(offerBanner);
                    repositoryWrapper.Save();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", ex.Message);
                    return View(offerBannerViewModel);
                }
                return RedirectToAction("Index");
            }
            return View(offerBannerViewModel);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var offerBannerToDelete = repositoryWrapper.OfferBanner.FindByCondition(x => x.Id == id);
            if (offerBannerToDelete.Any())
            {
                repositoryWrapper.OfferBanner.Delete(offerBannerToDelete.Single());
                repositoryWrapper.Save();
                return Json("Success");
            }
            Response.StatusCode = 404;
            return Json("Offer banner not found");
        }
    }
}