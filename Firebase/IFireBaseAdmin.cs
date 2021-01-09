using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invitee.Firebase
{
    public interface IFireBaseAdmin
    {
        Task<bool> SendNotificationAsync(string data, List<string> registrationTokens = null, string topicId = "");
        //Task<bool> SendNotificationAsync(NotificationModel notificationModel, List<string> registrationTokens = null, string topicId = "");
    }
}
