using Invitee.Entity;
using Invitee.Repository.Infra;

namespace Invitee.Repository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void UpdateChildCategoriesById(int[] ids, int parentCategoryId);
    }
}