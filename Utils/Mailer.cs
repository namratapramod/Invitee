using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Invitee.Utils
{
    public class Mailer
    {
        private static readonly EmailAddress FromEmailAddress = new EmailAddress(AppSettings.FromMailId, AppSettings.FromMailName);
        public static async Task Execute()
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("tech@balendin.in", "Balendin Tech");
            var subject = "Test Mail from SendGrid";
            var to = new EmailAddress("mthevar@balendin.in", "Muthukumar");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
        public static async Task<bool> SendEmail(string toAddress, string subject, string htmlContent)
        {
            try
            {
                var client = new SendGridClient(AppSettings.SendGridApiKey);
                var msg = MailHelper.CreateSingleEmail(FromEmailAddress, new EmailAddress(toAddress), subject, "", htmlContent);
                var response = await client.SendEmailAsync(msg);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}