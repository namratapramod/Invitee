﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Invitee.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LogsController : Controller
    {
        // GET: Logs
        public ActionResult Index()
        {
            return View();
        }
    }
}