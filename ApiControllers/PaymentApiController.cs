using Invitee.Infrastructure;
using Invitee.Infrastructure.PaymentInfra;
using Invitee.Models.Payment;
using Serilog;
using System.Web.Http;


namespace Invitee.ApiControllers
{
    [ApiAuthorize]
    public class PaymentApiController : BaseApiController
    {
        private readonly ILogger logger;
        private readonly IPaymentService paymentService;
        //public ICPaymentService PaymentService { get; set; }

        public PaymentApiController(ILogger logger, IPaymentService paymentService)
        {
            this.logger = logger.ForContext(this.GetType());
            this.paymentService = paymentService;
            //this.paymentService = paymentService;
        }

     
        [HttpPost]
        public async System.Threading.Tasks.Task<IHttpActionResult> GetCfToken(PaymentRequest paymentRequest)
        {
            logger.Information($"GetCftoken called by {GetUserName()}");

            var result = await this.paymentService.GetCftoken(paymentRequest);

            //if (result.ToLower().Contains("error"))
            //    return Error(result);
            return Success(new { AccessCode = result });
        }

        [HttpGet]
        public IHttpActionResult UpdatePaymentStatus(int id, bool status)
        {
            logger.Information($"UpdatePaymentStatus called by {GetUserName()}");

            var result = this.paymentService.UpdatePaymentStatus(id, status);

            if(result)
                return Success(new { Message = "Status Updated Successfully"});
            return NotFound();
        }
    }
}