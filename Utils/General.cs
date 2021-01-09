using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.Utils
{
    public static class General
    {
        public static string GetRandomString()
        {
            var random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[5];
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            var finalString = new String(stringChars);
            return finalString;
        }
    }
}