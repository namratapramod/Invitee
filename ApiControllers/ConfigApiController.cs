using Invitee.Entity;
using Invitee.Infrastructure;
using Invitee.Repository;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Invitee.ApiControllers
{
    public class ConfigApiController : BaseApiController
    {
        private readonly IRepositoryWrapper repositoryWrapper;
        private readonly ILogger logger;
        public ConfigApiController(IRepositoryWrapper repositoryWrapper, ILogger logger)
        {
            this.repositoryWrapper = repositoryWrapper;
            this.logger = logger.ForContext(this.GetType());
        }

        //[System.Web.Http.HttpPost]
        //public IHttpActionResult SaveConfig(Config token)
        //{
        //    try
        //    {
        //        repositoryWrapper.Config.Update(token);
        //        return Success(true);
        //    }
        //    catch(Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetConfig()
        {
            return Success(repositoryWrapper.Config.FindAll().ToList());
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult GetCurrentDateAndTime()
        {
            return Success(DateTime.Now);
        }
    }
}