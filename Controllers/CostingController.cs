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
    public class CostingController : Controller
    {
        private readonly IRepositoryWrapper repositoryWrapper;
        public CostingController(IRepositoryWrapper repositoryWrapper)
        {
            this.repositoryWrapper = repositoryWrapper;
        }
        // GET: Costing
        public ActionResult Index()
        {
            var result = repositoryWrapper.Costing.FindAll();
            return View(Mapper.Map<IEnumerable<CostingViewModel>>(result));
        }

        public ActionResult Create()
        {
            return View(new CostingViewModel());
        }

        [HttpPost]
        public ActionResult Create(CostingViewModel costingViewModel)
        {
            if (ModelState.IsValid)
            {
                var costing = Mapper.Map<Costing>(costingViewModel);
                var result = repositoryWrapper.Costing.Create(costing);
                repositoryWrapper.Save();
                ViewBag.Success = true;
            }
            return View(costingViewModel);
        }

        public ActionResult Edit(int? id)
        {
            var costing = repositoryWrapper.Costing.FindByCondition(x => x.Id == id);
            if (!costing.Any())
                return HttpNotFound();
            var costingModel = Mapper.Map<CostingViewModel>(costing.Single());
            return View(costingModel);
        }

        [HttpPost]
        public ActionResult Edit(CostingViewModel costingViewModel)
        {
            if (ModelState.IsValid)
            {
                var costing = Mapper.Map<Costing>(costingViewModel);
                var result = repositoryWrapper.Costing.Update(costing);
                repositoryWrapper.Save();
                ViewBag.Success = true;
            }
            return View(costingViewModel);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var costingToDelete = repositoryWrapper.Costing.FindByCondition(x => x.Id == id);
            if (costingToDelete.Any())
            {
                if (costingToDelete.Single().MediaTemplates.Where(x => x.IsDeleted == false).Any())
                    throw new Exception("Category is being used by  mediatemplate");
                //return Json(new { success = false, message = "This costing is being used by a mediatemplate." });
                repositoryWrapper.Costing.Delete(costingToDelete.Single());
                repositoryWrapper.Save();
                return Json(new { success = true });
            }
            Response.StatusCode = 404;
            return Json("Costing not found");
        }
    }
}