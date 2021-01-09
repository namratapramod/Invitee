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
using System.Web.Http;
using System.Web.Mvc;

namespace Invitee.Controllers
{
    [System.Web.Mvc.Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly IRepositoryWrapper repositoryWrapper;
        public CategoryController(IRepositoryWrapper repositoryWrapper)
        {
            this.repositoryWrapper = repositoryWrapper;
        }
        // GET: Category
        public ActionResult Index()
        {
            var categoryList = repositoryWrapper.Category.FindAll().ToList();
            var categoryModelList = Mapper.Map<List<CategoryViewModel>>(categoryList);
            ViewBag.CategoryList = categoryList.ToDictionary(x => x.Id, x => x.Name);
            repositoryWrapper.Config.GetReferralSettingsList();
            return View(categoryModelList);
        }

        public ActionResult Create()
        {
            return View(new CategoryViewModel()
            {
                ParentCategories = repositoryWrapper.Category.FindAll().OrderBy(x =>x.Name).ToList()
            });
        }
        // POST: Category
        [System.Web.Mvc.HttpPost]
        public ActionResult Create(CategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (categoryViewModel.File != null)
                    {
                        SaveOrUpdateCategoryImageFile(categoryViewModel);
                    }
                    var category = Mapper.Map<Category>(categoryViewModel);
                    repositoryWrapper.Category.Create(category);
                    repositoryWrapper.Save();
                    ViewBag.Success = true;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", ex.Message);
                }
            }
            categoryViewModel.ParentCategories = repositoryWrapper.Category.FindAll().ToList();
            return View(categoryViewModel);
        }

        private void SaveOrUpdateCategoryImageFile(CategoryViewModel categoryViewModel)
        {
            categoryViewModel.File.ValidateImageFile();
            //Delete existing file if exists 
            if (!string.IsNullOrEmpty(categoryViewModel.ImageUrl) && System.IO.File.Exists(Server.MapPath($"~/{categoryViewModel.ImageUrl}")))
            {
                System.IO.File.Delete(Server.MapPath($"~/{categoryViewModel.ImageUrl}"));
            }
            var newFileName = categoryViewModel.File.GetNewFileName();
            categoryViewModel.ImageUrl = $"{AppSettings.CategoryImagePath}{newFileName}";
            if (!Directory.Exists(Server.MapPath($"~/{AppSettings.CategoryImagePath}")))
                Directory.CreateDirectory(Server.MapPath($"~/{AppSettings.CategoryImagePath}"));
            categoryViewModel.File.SaveAs(Path.Combine(Server.MapPath($"~/{AppSettings.CategoryImagePath}"), newFileName));
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult UpdateChildCategories([FromBody] int[] childCategories, int parentCategory)
        {
            repositoryWrapper.Category.UpdateChildCategoriesById(childCategories, parentCategory);
            repositoryWrapper.Save();
            return Json("Success");
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Delete(int id)
        {
            var catgToDelete = repositoryWrapper.Category.FindByCondition(x => x.Id == id);
            if (catgToDelete.Any())
            {
                if (repositoryWrapper.MediaTemplate.FindByCondition(x=>x.CategoryId == id && x.IsDeleted==false).Any())
                    throw new Exception("Category is being used by  mediatemplate");
                repositoryWrapper.Category.Delete(catgToDelete.SingleOrDefault());
                repositoryWrapper.Save();
            }
            return Json("Success");
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult Edit(int? id)
        {
            var category = repositoryWrapper.Category.FindByCondition(x => x.Id == id);
            if (!category.Any())
                return HttpNotFound();
            var categoryModel = Mapper.Map<CategoryViewModel>(category.Single());
            categoryModel.ParentCategories = repositoryWrapper.Category.FindByCondition(x=>x.Id!=id).OrderBy(x =>x.Name).ToList();
            return View(categoryModel);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Edit(CategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (categoryViewModel.File != null)
                    {
                        SaveOrUpdateCategoryImageFile(categoryViewModel);
                    }
                    var category = Mapper.Map<Category>(categoryViewModel);
                    repositoryWrapper.Category.Update(category);
                    repositoryWrapper.Save();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", ex.Message);
                    return View(categoryViewModel);
                }
                return RedirectToAction("Index");
            }
            return View(categoryViewModel);
        }
    }
}