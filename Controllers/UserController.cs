using AutoMapper;
using Invitee.Repository;
using Invitee.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Invitee.Controllers
{
    [System.Web.Mvc.Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IRepositoryWrapper repositoryWrapper;
        public UserController(IRepositoryWrapper repositoryWrapper)
        {
            this.repositoryWrapper = repositoryWrapper;
        }

        // GET: User
        public ActionResult Index()
        {
            var user = Mapper.Map<IEnumerable<UserViewModel>>(repositoryWrapper.User.FindByCondition(x=>x.UserRole!= Entity.Enums.UserRole.Admin));
            return View(user);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var user = repositoryWrapper.User.FindByCondition(x => x.Id == id).SingleOrDefault();
            if (user != null)
            {
                repositoryWrapper.User.Delete(user); 
                repositoryWrapper.Save(); 
                return Json("Success");
            }
            return HttpNotFound();
        }

        public ActionResult Comments()
        {
            var data = repositoryWrapper.Delivery.FindAll().ToList();
            return View(data);
        }
    }
}