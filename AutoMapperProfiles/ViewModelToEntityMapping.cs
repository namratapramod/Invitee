using AutoMapper;
using Invitee.Entity;
using Invitee.ViewModels;

namespace Invitee.AutoMapperProfiles
{
    public class ViewModelToEntityMapping : Profile
    {
        public ViewModelToEntityMapping()
        {
            CreateMap<CategoryViewModel, Category>();
            CreateMap<UserViewModel, User>().ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.HashedPassword));
            CreateMap<CostingViewModel, Costing>();
            CreateMap<ImageCostViewModel, ImageCost>();
            CreateMap<MediaTemplateViewModel, MediaTemplate>();
            CreateMap<OrderViewModel, Order>();
            CreateMap<DeliveryViewModel, Delivery>();
            CreateMap<OfferBannerViewModel, OfferBanner>();
            CreateMap<FilterViewModel, MediaFilter>();
        }
     }
}