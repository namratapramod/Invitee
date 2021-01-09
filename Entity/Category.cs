using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Invitee.Entity
{
    public class Category : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(60)]
        public string Name { get; set; }
        
        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public int? ParentCategoryId { get; set; }
        public virtual Category ParentCategory { get; set; }

        [JsonIgnore]
        public virtual ICollection<Category> ChildCategories { get; set; }
        public bool IsDefault { get; set; } = false;
        public string ExtraInputOne { get; set; }
        public string ExtraInputTwo { get; set; }
    }
}