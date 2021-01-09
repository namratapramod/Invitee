using Invitee.Entity;

namespace Invitee.Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private RepositoryContext _repositoryContext;
        private IUserRepository _user;
        private ICategoryRepository _category;
        private IImageCostRepository _imageCost;
        private ICostingRepository _costing;
        private IMediaTemplateRepository _mediaTemplate;
        private IOrderRepository _order;
        private IDeliveryRepository _delivery;
        private IOfferBannerRepository _offerBanner;
        private IFilterRepository _filter;
        private IConfigRepository _config;
        private IUploadedFilesRepository _uploadedFiles;
        private IPaymentRepository _paymentRepository;
        private ICashfreePaymentRepository _cashfreePaymentRepository;
        public RepositoryWrapper(RepositoryContext repositoryContext)
        {
            this._repositoryContext = repositoryContext;
            this._user = new UserRepository(_repositoryContext);
            this._category = new CategoryRepository(_repositoryContext);
            this._imageCost = new ImageCostRepository(_repositoryContext);
            this._costing = new CostingRepository(_repositoryContext);
            this._mediaTemplate = new MediaTemplateRepository(_repositoryContext);
            this._order = new OrderRepository(_repositoryContext);
            this._delivery = new DeliveryRepository(_repositoryContext);
            this._offerBanner = new OfferBannerRepository(_repositoryContext);
            this._filter = new FilterRepository(_repositoryContext);
            this._config = new ConfigRepository(_repositoryContext);
            this._uploadedFiles = new UploadedFilesRepository(_repositoryContext);
            this._paymentRepository = new PaymentRepository(_repositoryContext);
            this._cashfreePaymentRepository = new CashfreePaymentRepository(_repositoryContext);
        }
        public IUserRepository User => _user;
        public ICategoryRepository Category => _category;
        public ICostingRepository Costing => _costing;
        public IImageCostRepository ImageCost => _imageCost;
        public IMediaTemplateRepository MediaTemplate => _mediaTemplate;
        public IOrderRepository Order => _order;
        public IDeliveryRepository Delivery => _delivery;
        public IOfferBannerRepository OfferBanner => _offerBanner;
        public IFilterRepository MediaFilter => _filter;
        public IConfigRepository Config => _config;
        public IUploadedFilesRepository UploadedFiles => _uploadedFiles;
        public IPaymentRepository Payment => _paymentRepository;
        public ICashfreePaymentRepository CashfreePayment => _cashfreePaymentRepository;

        public void Save()
        {
            _repositoryContext.SaveChanges();
        }  
    }
}