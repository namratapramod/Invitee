using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Invitee.Models
{
    public class UserLoginModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }
        [DefaultValue(false)]
        public bool IsGoogleSignIn { get; set; }
    }
}