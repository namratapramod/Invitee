using Invitee.Entity;
using Invitee.Repository.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.Repository
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        private RepositoryContext repositoryContext;
        public CategoryRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            this.repositoryContext = repositoryContext;
        }

        public void UpdateChildCategoriesById(int[] ids, int parentCategoryId)
        {
            var dbCatg = repositoryContext.Categories.Find(parentCategoryId);
            if (dbCatg != null)
            {
                dbCatg.ChildCategories.Clear();
                if (ids == null)
                    return;
                var childCatgs = repositoryContext.Categories.Where(x => ids.Contains(x.Id));
                foreach (var item in childCatgs)
                {
                    repositoryContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    item.ParentCategoryId = parentCategoryId;
                }
            }
        }
    }
}