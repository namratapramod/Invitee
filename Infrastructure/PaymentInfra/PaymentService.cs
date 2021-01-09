using Invitee.Entity;
using Invitee.Models.Payment;
using Invitee.Repository;
using System;
using System.Linq;
using Flurl.Http;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Invitee.Utils;

namespace Invitee.Infrastructure.PaymentInfra
{
    public class PaymentService : IPaymentService
    {
       

        private readonly IRepositoryWrapper repositoryWrapper;

        public PaymentService(IRepositoryWrapper repositoryWrapper)
        {
            this.repositoryWrapper = repositoryWrapper;
          
        }

        #region CCAvenue Code Methods
        // Below method are required for ccavenue payment integration

        /***
        [Obsolete]
        public async Task<string> GetRSAKeyAsync(int id)
        {
            try
            {
                //"https://test.ccavenue.com/transaction/getRSAKey"
                string queryUrl = this.activePaymentConfig.RSAKeyUrl;
                //string vParams = $"access_code={this.activePaymentConfig.AccessCode}&workingKey={this.activePaymentConfig.WorkingKey}";

                // Url Connection
                String message = await postPaymentRequestToGateway(queryUrl, id, this.activePaymentConfig.AccessCode, this.activePaymentConfig.WorkingKey);
                return message;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
        [Obsolete]
        private async Task<string> postPaymentRequestToGateway(String queryUrl, int orderId, string accessCode, string workingKey)
        {
            string message;
            try
            {
                message = await queryUrl
                    .PostUrlEncodedAsync(new { access_code = accessCode, workingKey = workingKey, order_id = orderId })
                    .ReceiveString();
            }
            catch (Exception exception)
            {
                Console.Write("Exception occured while connection." + exception);
                message = "Error: " + exception.Message;
            }
            return message;
        }
        [Obsolete]
        public CCAvenueRawResponse SubmitResponse(string encResp)
        {
            string workingKey = this.activePaymentConfig.WorkingKey;
            CCACrypto ccaCrypto = new CCACrypto();
            string encResponse = ccaCrypto.Decrypt(encResp, workingKey);
            NameValueCollection Params = new NameValueCollection();
            string[] segments = encResponse.Split('&');
            CCAvenueRawResponse rawResponse = new CCAvenueRawResponse();
            foreach (string seg in segments)
            {
                string[] parts = seg.Split('=');
                if (parts.Length > 0)
                {
                    string Key = parts[0].Trim();
                    string Value = parts[1].Trim();
                    rawResponse.GetType().GetProperty(Key).SetValue(rawResponse, Value);
                    //Params.Add(Key, Value);
                }
            }
            return rawResponse;
        }
        *************/
        #endregion

        /// <summary>
        /// Get CF token and update request in database.
        /// </summary>
        /// <param name="paymentRequest"></param>
        /// <returns></returns>
        public async Task<PaymentResponce> GetCftoken(PaymentRequest paymentRequest)
        {
            var response =  await AppSettings.CASHFREE_URL
                 .WithHeaders(new { x_client_id = AppSettings.CASHFREE_APP_ID, x_client_secret = AppSettings.CASHFREE_SECRET_KEY }, true)
                 .PostJsonAsync(paymentRequest)
                 .ReceiveJson<PaymentResponce>();

            //Add the log to the Cashfree table 
            repositoryWrapper.Payment.Create(new CashfreePayment
            {
                OrderId = Convert.ToInt32(paymentRequest.orderId),
                Currency = paymentRequest.orderCurrency,
                OrderAmount = Convert.ToDouble(paymentRequest.orderAmount),
                Status = response.Status == "OK",
                CfToken = response.Cftoken,
                CreatedDate = DateTime.Now
            });
            repositoryWrapper.Save();
            return response;
        }
        /// <summary>
        /// update payment status in database
        /// </summary>
        /// <param name="paymentResponce"></param>
        /// <returns></returns>
        public bool UpdatePaymentStatus(int orderId, bool status)
        {
            var record = this.repositoryWrapper.Payment.FindAll(true).Where(x => x.OrderId == orderId).OrderByDescending(o => o.CreatedDate).FirstOrDefault();
            if (record != null)
            {
                record.Status = status;
                this.repositoryWrapper.Save();
                return true;
            }
            return false;
        }
    }
}