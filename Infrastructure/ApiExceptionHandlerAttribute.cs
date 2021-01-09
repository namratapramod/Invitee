using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using Serilog;
using System.Web.Http.Filters;
using Autofac;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;

namespace Invitee.Infrastructure
{
    public class ApiExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger logger;
        public ApiExceptionHandlerAttribute()
        {
            var log = DependencyResolver.Current.GetService(typeof(ILogger)) as ILogger;
            this.logger = log.ForContext(this.GetType());
        }
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {

            var exType = actionExecutedContext.Exception.GetType();

            if (exType == typeof(DbEntityValidationException))
            {
                var errorMessages = ((DbEntityValidationException)actionExecutedContext.Exception).EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);
                actionExecutedContext.Exception = new DbEntityValidationException(string.Join("\n", errorMessages), ((DbEntityValidationException)actionExecutedContext.Exception).EntityValidationErrors);
            }
            else if(exType == typeof(DbUpdateException))
            {
                var errorMessage = ((DbUpdateException)actionExecutedContext.Exception).InnerException;
                actionExecutedContext.Exception = new Exception(errorMessage.Message.Contains("inner exception")?errorMessage.InnerException.Message : errorMessage.Message);
            }
            logger.Error(actionExecutedContext.Exception, "Global Exception Handled - Action Name " + actionExecutedContext.ActionContext.ActionDescriptor.ActionName);
            base.OnException(actionExecutedContext);
        }
    }
}