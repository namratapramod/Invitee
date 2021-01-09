using Invitee.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Invitee.ViewModels
{
    public class CategoryViewModel
    {
        public CategoryViewModel()
        {
            this.ChildCategories = new List<Category>();
            this.ParentCategories = new List<Category>();
        }
        public int Id { get; set; }

        [Required(ErrorMessage = "Category Name is required")] 
        [MaxLength(60)]
        public string Name { get; set; }

        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public int? ParentCategoryId { get; set; }
        public virtual Category ParentCategory { get; set; }

        public virtual List<Category> ChildCategories { get; set; }
        public bool IsDefault { get; set; } = false;

        public HttpPostedFileBase File { get; set; }

        public List<Category> ParentCategories { get; set; }
        public string ExtraInputOne { get; set; }
        public string ExtraInputTwo { get; set; }
    }
}