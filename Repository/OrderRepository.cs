using AutoMapper;
using Invitee.Entity;
using Invitee.Repository.Infra;
using Invitee.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.Repository
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        private RepositoryContext repositoryContext;
        public OrderRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            this.repositoryContext = repositoryContext;
        }

        public void AddOrderImage(OrderImage order)
        {
            repositoryContext.OrderImages.Add(order);
        }

        Order IOrderRepository.Create(Order order)
        {
            foreach (var item in order.MediaFilters)
            {
                repositoryContext.Entry(item).State = System.Data.Entity.EntityState.Modified;
            }
            return repositoryContext.Orders.Add(order);
        }

        public void AddUpdateCommentAndRating(string comment, int rating, Guid id)
        {
            var delivery = this.repositoryContext.Deliveries.Where(x => x.Id == id).SingleOrDefault();
            if (delivery != null)
            {
                delivery.Comment = comment;
                delivery.Rating = rating;
            }
            else
            {
                throw new Exception($"Delivery with id {id} not available!");
            }
        }

        public void LikeUnlike(Guid id, int userId)
        {
            var existing = this.repositoryContext.DeliveryLikes.Where(x => x.DeliveryId == id && x.UserId == userId);
            if (existing.Any())
                this.repositoryContext.Entry(existing.Single()).State = System.Data.Entity.EntityState.Deleted;
            else
                this.repositoryContext.DeliveryLikes.Add(new DeliveryLike { DeliveryId = id, UserId = userId });
        }

        public IQueryable<Delivery> ListDeliveries(int userId)
        {
            return repositoryContext.Deliveries.Where(x => x.Order.UserId == userId);
        }

        public IQueryable<Delivery> ListDeliveries()
        {
            return repositoryContext.Deliveries.Where(x => x.IsPublic == true);
        }

        public void MakePublic(Guid id, int userId)
        {
            var delivery = repositoryContext.Deliveries.Where(x => x.Id == id && x.Order.UserId == userId).FirstOrDefault();
            if (delivery != null)
            {
                delivery.IsPublic = true;
            }
            else
            {
                throw new InvalidOperationException($"User id {userId} not available for this video to make public!");
            }
            
        }
    }
}