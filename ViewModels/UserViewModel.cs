using Invitee.Entity;
using Invitee.Entity.Enums;
using Invitee.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Invitee.ViewModels
{
    public class UserViewModel : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string  FullName { get; set; }

        [Required]
        [MaxLength(12)]
        //[RegularExpression("(0/91)?[1-9][0-9]{9}", ErrorMessage = "Invalid mobile number")]
        public string Mobile { get; set; }

        [Required]
        [MaxLength(150)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public bool IsMobileVerified { get; set; } = false;

        public DateTime? DateOfBirth { get; set; }
        public string Country { get; set; }

        [MaxLength(100)]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [EnumDataType(typeof(Gender))]
        public Gender? Gender { get; set; }

        public bool IsEmailVerified { get; set; } = false;

        public string ProfilePic { get; set; }

        [EnumDataType(typeof(UserRole))]
        public UserRole UserRole { get; internal set; }

        public string DeviceId { get; set; } //This is required for sending the push notifications

        [MaxLength(60)]
        public string ReferralCodeToBeShared { get; set; }
        public string ReferralCodeApplied { get; set; }
        public int EarnedVideo { get; set; }
        public int ClaimedVideo { get; set; }
        public string HashedPassword
        {
            get
            {
                return Encryption.HashString(Password);
            }
        }
        [DefaultValue(false)]
        public bool IsGoogleSignIn { get; set; }
    }
}