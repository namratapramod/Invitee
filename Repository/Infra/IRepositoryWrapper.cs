using Invitee.Entity;
using Invitee.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.Repository
{
    public interface IRepositoryWrapper
    {
        IUserRepository User { get; }
        ICategoryRepository Category { get; }
        ICostingRepository Costing { get; }
        IImageCostRepository ImageCost { get; }
        IMediaTemplateRepository MediaTemplate { get; }
        IOrderRepository Order { get; }
        IDeliveryRepository Delivery { get; }
        IOfferBannerRepository OfferBanner { get; }
        IFilterRepository MediaFilter { get; }
        IConfigRepository Config { get; }
        IUploadedFilesRepository  UploadedFiles { get; }
        IPaymentRepository Payment { get; }
        ICashfreePaymentRepository CashfreePayment { get; }
        void Save();
    }
}