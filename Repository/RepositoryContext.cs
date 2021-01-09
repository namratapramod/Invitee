using Invitee.Entity;
using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;

namespace Invitee.Repository
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class RepositoryContext : DbContext
    {
        public RepositoryContext()
            : base("DefaultContext")
        {
        }
        public IQueryable Set(string name)
        {
            return base.Set(Type.GetType($"Invitee.Entity.{name}")).AsQueryable();
        }
        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted));
            foreach (var item in entries)
            {
                if (item.State == EntityState.Modified)
                {
                    ((BaseEntity)item.Entity).UpdatedDate = DateTime.Now;
                }
                else if (item.State == EntityState.Added)
                {
                    ((BaseEntity)item.Entity).CreatedDate = DateTime.Now;
                }
                else if (item.State == EntityState.Deleted)
                {
                    if ((item.Entity as ISoftDelete) !=null)
                    {
                        item.State = EntityState.Modified;
                        ((ISoftDelete)item.Entity).IsDeleted = true;
                    }
                }
            }
            return base.SaveChanges();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasOptional(u => u.ParentCategory)
                .WithMany(u => u.ChildCategories)
                .HasForeignKey(u => u.ParentCategoryId);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Costing> Costings { get; set; }
        public DbSet<ImageCost> ImageCosts { get; set; }
        public DbSet<MediaTemplate> MediaTemplates { get; set; }
        public DbSet<SlideText> SlideTexts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderImage> OrderImages { get; set; }
        public DbSet<OfferBanner> offerBanners { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Config> Configs { get; set; }
        public DbSet<MediaFilter> MediaFilters { get; set; }
        public DbSet<MediaTemplateLike> MediaTemplateLikes { get; set; }
        public DbSet<DeliveryLike> DeliveryLikes { get; set; }
        public DbSet<ReferralSettings> ReferralSettings { get; set; }
        public DbSet<PaymentConfig> PaymentConfigs { get; set; }
        public DbSet<CashfreePayment> CashfreePayments { get; set; }
    }
}