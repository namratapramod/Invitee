using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Invitee.Utils
{
    public static class ControllerExtension
    { 
        public static IList<string> BuildModelError(this Controller controller)
        {
            var erros = new List<String>();
            foreach (ModelState modelState in controller.ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    erros.Add(error.ErrorMessage);
                }
            }
            return erros;
        }
    }
}