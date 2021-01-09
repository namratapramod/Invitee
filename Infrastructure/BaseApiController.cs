using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace Invitee.Infrastructure
{
    public class BaseApiController : ApiController
    {
        public BaseApiController()
        {

        }
        protected IHttpActionResult CreateResponse<T>(T content)
        {
            if (!ModelState.IsValid)
                return ModelError();
            else
                return Success(content);
        }

        protected int GetUserId()
        {
            return Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.SingleOrDefault(x => x.Type == "Id").Value);
        }

        protected string GetDeviceId(Repository.IRepositoryWrapper repositoryWrapper)
        {
            var userId = this.GetUserId();
            return repositoryWrapper.User.FindAll().Single(x => x.Id == userId).DeviceId;
        }

        protected string GetUserName()
        {
            return ((ClaimsIdentity)User.Identity).Name;
        }

        protected IHttpActionResult ModelError()
        {
            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
        }

        protected IHttpActionResult Error(string message)
        {
            return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest,message));
        }

        protected IHttpActionResult Success<T>(T content)
        {
            return Ok(content);
        }
    }
}