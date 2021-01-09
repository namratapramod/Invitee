using Invitee.Entity.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Invitee.Entity
{
    public class User : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        [MaxLength(12)]
        [Index("IX_MobileNumber", IsUnique = true)]
        public string Mobile { get; set; }

        [Required]
        [Index("IX_Username", IsUnique = true)]
        [MaxLength(150)]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public bool IsMobileVerified { get; set; } = false;

        public DateTime? DateOfBirth { get; set; }
        public string Country { get; set; }

        [Index("IX_Email", IsUnique = true)]
        [MaxLength(100)]
        public string Email { get; set; }

        [EnumDataType(typeof(Gender))]
        public Gender? Gender { get; set; }

        public bool IsEmailVerified { get; set; } = false;

        public string ProfilePic { get; set; }

        [EnumDataType(typeof(UserRole))]
        public UserRole UserRole { get; internal set; }

        public string DeviceId { get; set; } //This is required for sending the push notifications

        [Index("IX_ReferralCodeToBeShared", IsUnique = true)]
        [MaxLength(60)]
        public string ReferralCodeToBeShared { get; set; }
        public string ReferralCodeApplied { get; set; }
        public bool IsDeleted { get; set; }
        public int EarnedVideo { get; set; }
        public int ClaimedVideo { get; set; }
        public int ReferralIncrementer { get; set; }
        [DefaultValue(false)]
        public bool IsGoogleSignIn { get; set; }
    }
}