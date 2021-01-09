using Invitee.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Invitee.ViewModels
{
    public class ChangePasswordModel : BaseEntity
    {
        public int Id { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Confirm password does not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}