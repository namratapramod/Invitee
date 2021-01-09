using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Invitee.Utils
{
    public static class AppSettings
    {
        public static string EncrKey { get; set; } = ConfigurationManager.AppSettings["EncryptionKey"];
        public static string FireBaseKey { get; set; } = ConfigurationManager.AppSettings["FireBaseKey"];
        public static string VideoFilePath { get; } = "Videos/TemplateVideos/";
        public static string VideoThumnailFilePath { get; } = "Images/VideoTemplateThumbnails/";
        public static string ProfilePicturesPath { get; } = "Images/ProfilePictures/";
        public static string SendGridApiKey { get; set; } = ConfigurationManager.AppSettings["SendGridApiKey"];
        public static string CategoryImagePath { get; } = "Images/CategoryImages/";
        public static string DeliveryFilePath { get; } = "Videos/Delivery/";
        public static string OfferBannerImagesPath { get; } = "Images/OfferBannerImages/";
        public static string OrderImagePath { get; } = "Images/OrderImages/";
        public static string OrderAudioPath { get; } = "Images/OrderAudios/";
        public static string DeliveryThumnailFilePath { get; } = "Images/DeliveryThumnailFileImages/";
        public static string ComplementaryThumnailFilePath { get; } = "Images/DeliveryThumnailFileImages/";
        public static string CASHFREE_APP_ID { get; } = ConfigurationManager.AppSettings["CASHFREE_APP_ID"];
        public static string CASHFREE_SECRET_KEY { get; } = ConfigurationManager.AppSettings["CASHFREE_SECRET_KEY"];
        public static string CASHFREE_URL { get; } = ConfigurationManager.AppSettings["CASHFREE_URL"];
        public static string FromMailId { get; } = "loquininvitee@gmail.com";
        public static string FromMailName { get; } = "Invitee";
        public static string CompleteOrderUrl {get;} = "https://play.google.com/store/apps/details?id=com.invitee";
    }
}