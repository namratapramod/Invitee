using Invitee.Entity;
using Invitee.Repository;
using Invitee.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Invitee.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ConfigController : Controller
    {
        private readonly RepositoryContext repositoryContext;
        private readonly IRepositoryWrapper repositoryWrapper;
        public ConfigController(RepositoryContext repositoryContext, IRepositoryWrapper repositoryWrapper)
        {
            this.repositoryContext = repositoryContext;
            this.repositoryWrapper = repositoryWrapper;
        }
        // GET: Config
        public ActionResult Index()
        {
            var result = repositoryContext.Configs.FirstOrDefault();
            return View(result);
        }

        [HttpPost]
        public ActionResult Create(Config config)
        {
            var dbConfig = repositoryContext.Configs.First();
            dbConfig.FireBaseToken = config.FireBaseToken;
            dbConfig.FreeVideoOnRegister = config.FreeVideoOnRegister;
            dbConfig.PlaystoreVersionNumber = config.PlaystoreVersionNumber;
            repositoryContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult ReferralSettingList()
        {
            var data = repositoryWrapper.Config.GetReferralSettingsList();
            return View(data);
        }
        public ActionResult CreateReferralSetting()
        {
            return View("CreateReferralSetting");
        }
        [HttpPost]
        public ActionResult CreateReferralSetting(ReferralSettings referralSettings)
        {
            repositoryWrapper.Config.AddReferralSettings(referralSettings);
            repositoryContext.SaveChanges();
            return RedirectToAction("ReferralSettingList");
        }
    }
}