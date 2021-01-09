using Flurl.Http;
using Invitee.Repository;
using Invitee.Utils;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Invitee.Firebase
{
    public class FireBaseAdmin : IFireBaseAdmin
    {
        private const string FireBaseNotificationURL = "https://fcm.googleapis.com/fcm/send";
        private readonly IRepositoryWrapper repositoryWrapper;
        private readonly ILogger logger;
        public FireBaseAdmin(IRepositoryWrapper repositoryWrapper, ILogger logger)
        {
            this.repositoryWrapper = repositoryWrapper;
            this.logger = logger;
           
        }

        public async Task<bool> SendNotificationAsync(string data, List<string> registrationTokens = null, string topicId = "")
        {
           
            try
            {
                var result = await FireBaseNotificationURL
                                .WithHeader("Authorization", $"key={AppSettings.FireBaseKey}")
                                .PostJsonAsync(new
                                {
                                    to = string.IsNullOrEmpty(topicId) ? registrationTokens[0] : $"/topics/{topicId}",
                                    priority = "high",
                                    notification = new
                                    {
                                        body = data,
                                        title = "Invitee",
                                        badge = 1
                                    }
                                })
                                .ReceiveJson();
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occured while sending notification : SendNotificationAsync");
                return false;
            }
        }


        //public async Task<bool> SendNotificationAsync(NotificationModel notificationModel, List<string> registrationTokens = null, string topicId = "")
        //{
        //    if (registrationTokens == null)
        //    {
        //        var configRepository = new ConfigRepository();
        //        registrationTokens = new List<string> { configRepository.List.First().FireBaseToken };
        //    }
        //    try
        //    {
        //        var result = await FireBaseNotificationURL
        //                        .WithHeader("Authorization", $"key={AppSettings.FireBaseKey}")
        //                        .PostJsonAsync(new
        //                        {
        //                            to = string.IsNullOrEmpty(topicId) ? registrationTokens[0] : $"/topics/{topicId}",
        //                            priority = "high",
        //                            notification = new
        //                            {
        //                                body = notificationModel,
        //                                title = "TimePaas",
        //                                badge = 1
        //                            }
        //                        })
        //                        .ReceiveJson();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        //        return false;
        //    }
        //}
    }
}