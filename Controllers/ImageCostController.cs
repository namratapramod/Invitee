using AutoMapper;
using Invitee.Entity;
using Invitee.Repository;
using Invitee.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Invitee.Controllers
{
    [System.Web.Mvc.Authorize(Roles = "Admin")]
    public class ImageCostController : Controller
    {
        private readonly IRepositoryWrapper repositoryWrapper;
        public ImageCostController(IRepositoryWrapper repositoryWrapper)
        {
            this.repositoryWrapper = repositoryWrapper;
        }

        // GET: ImageCost
        public ActionResult Index()
        {
            var imageCosting = repositoryWrapper.ImageCost.FindAll();
            return View(Mapper.Map<IEnumerable<ImageCostViewModel>>(imageCosting));
        }
        public ActionResult Create()
        {
            return View(new ImageCostViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ImageCostViewModel imageCostViewModel)
        {
            if (ModelState.IsValid)
            {
                var imageCost = Mapper.Map<ImageCost>(imageCostViewModel);
                var result = repositoryWrapper.ImageCost.Create(imageCost);
                repositoryWrapper.Save();
                ViewBag.Success = true;
            }
            return View(imageCostViewModel);
        }

        public ActionResult Edit(int? id)
        {
            var imageCost = repositoryWrapper.ImageCost.FindByCondition(x => x.Id == id);
            if (!imageCost.Any())
                return HttpNotFound();
            var imageCostModel = Mapper.Map<ImageCostViewModel>(imageCost.Single());
            return View(imageCostModel);
        }

        [HttpPost]
        public ActionResult Edit(ImageCostViewModel imageCostViewModel)
        {
            if (ModelState.IsValid)
            {
                var imageCost = Mapper.Map<ImageCost>(imageCostViewModel);
                var result = repositoryWrapper.ImageCost.Update(imageCost);
                repositoryWrapper.Save();
                ViewBag.Success = true;
            }
            return View(imageCostViewModel);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var imageCostToDelete = repositoryWrapper.ImageCost.FindByCondition(x => x.Id == id);
            if (imageCostToDelete.Any())
            {
                repositoryWrapper.ImageCost.Delete(imageCostToDelete.Single());
                repositoryWrapper.Save();
                return Json("Success");
            }
            Response.StatusCode = 404;
            return Json("ImageCost not found");
        }
    }
}