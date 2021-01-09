using AutoMapper;
using Invitee.Entity;
using Invitee.Repository;
using Invitee.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace Invitee.Controllers
{
    [System.Web.Mvc.Authorize(Roles = "Admin")]
    public class FilterController : Controller
    {
        private readonly IRepositoryWrapper repositoryWrapper;
        public FilterController(IRepositoryWrapper repositoryWrapper)
        {
            this.repositoryWrapper = repositoryWrapper;
        }
        public ActionResult Index()
        {
            var filter = repositoryWrapper.MediaFilter.FindAll();
            return View(Mapper.Map<IEnumerable<FilterViewModel>>(filter));
        }
        public ActionResult Create()
        {
            return View(new FilterViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FilterViewModel filterViewModel)
        {
            if (ModelState.IsValid)
            {
                var filter = Mapper.Map<MediaFilter>(filterViewModel);
                var result = repositoryWrapper.MediaFilter.Create(filter);
                repositoryWrapper.Save();
                ViewBag.Success = true;
            }
            return View(filterViewModel);
        }
        public ActionResult Edit(int? id)
        {
            var filterData = repositoryWrapper.MediaFilter.FindByCondition(x => x.Id == id);
            if (!filterData.Any())
                return HttpNotFound();
            var filtetDataModel = Mapper.Map<FilterViewModel>(filterData.Single());
            return View(filtetDataModel);
        }
        [HttpPost]
        public ActionResult Edit(FilterViewModel filterViewModel)
        {
            if (ModelState.IsValid)
            {
                var filter = Mapper.Map<MediaFilter>(filterViewModel);
                var result = repositoryWrapper.MediaFilter.Update(filter);
                repositoryWrapper.Save();
                ViewBag.Success = true;
            }
            return View(filterViewModel);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var filterData = repositoryWrapper.MediaFilter.FindByCondition(x => x.Id == id);
            if (filterData.Any())
            {
                repositoryWrapper.MediaFilter.Delete(filterData.Single());
                repositoryWrapper.Save();
                return Json("Success");
            }
            Response.StatusCode = 404;
            return Json("filterData not found");
        }
    }
}