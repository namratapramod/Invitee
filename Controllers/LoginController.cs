using AutoMapper;
using Invitee.Models;
using Invitee.Repository;
using Invitee.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Invitee.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly IRepositoryWrapper repositoryWrapper;

        public LoginController(IRepositoryWrapper repositoryWrapper)
        {
            this.repositoryWrapper = repositoryWrapper;
        }
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminLogin(UserLoginModel userLoginModel)
        {
           
            if (ModelState.IsValid)
            {
                var user = repositoryWrapper.User.ValidateAdminUser(userLoginModel.UserName, userLoginModel.Password);
                if (user != null)
                {
                    //SessionManager.User = Mapper.Map<UserModel>(user);
                    FormsAuthentication.SetAuthCookie(user.UserName, false);
                    return RedirectToAction("Index", "Category");
                }
                else
                {
                    ViewBag.Message = "Invalid username or password !";
                }
            }
            return View("Index");
        }
        public ActionResult ForgetPassword()
        {
            ViewBag.Success = false;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ForgetPassword(UserLoginModel userLoginModel)
        {
            ViewBag.Success = false;
            if (ModelState.IsValid)
            {
                var user = repositoryWrapper.User.EmailExists(userLoginModel.Email);
                if (user != null)
                {
                    var newpass = Membership.GeneratePassword(8, 4);
                    user.Password = Encryption.HashString(newpass);
                    repositoryWrapper.User.UpdatePassword(user);
                    var result = await Mailer.SendEmail(userLoginModel.Email, "Your reset password", "Your reset Password is " + newpass);
                    ViewBag.Message = "New password has been generated and sent to your email address";
                    ViewBag.Success = true;
                }
                else
                {
                    ViewBag.Message = "Invalid email address!";
                }
            }
            else
            {
                ViewBag.Message = "Invalid email address!";
            }
            return View();
        }

        public void LogOut()
        {
            Session.RemoveAll();
            Session.Clear();
            FormsAuthentication.SignOut();
            Session.Abandon();
            FormsAuthentication.RedirectToLoginPage();
        }
    }
}