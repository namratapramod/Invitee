using AutoMapper;
using Invitee.Entity;
using Invitee.ViewModels;
using System.Linq;
namespace Invitee.AutoMapperProfiles
{
    public class EntityToViewModelMapping : Profile
    {
        public EntityToViewModelMapping()
        {
            CreateMap<Category, CategoryViewModel>();
            CreateMap<User, UserViewModel>().ForMember(x => x.FullName, m => m.MapFrom(src => src.FirstName + " " + src.LastName));
            CreateMap<Costing, CostingViewModel>();
            CreateMap<ImageCost, ImageCostViewModel>();
            CreateMap<MediaTemplate, MediaTemplateViewModel>().ForMember(x=>x.CategoryName, m => m.MapFrom(src=>src.Category.Name)).ForMember(x=>x.SlideTextInput , m=>m.MapFrom(src=>string.Join(",", src.SlideTexts.Select(x=>x.Text).ToArray())));
            CreateMap<Order, OrderViewModel>();
            CreateMap<OfferBanner, OfferBannerViewModel>();
            CreateMap<MediaFilter, FilterViewModel>();
        }
    }
}
