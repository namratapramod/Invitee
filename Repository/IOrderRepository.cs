using Invitee.Entity;
using Invitee.Repository.Infra;
using Invitee.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.Repository
{
    public interface IOrderRepository : IRepository<Order>
    {
        void AddOrderImage(OrderImage order);
        void MakePublic(Guid id, int userId);
        IQueryable<Delivery> ListDeliveries(int userId);
        IQueryable<Delivery> ListDeliveries();
        void LikeUnlike(Guid id, int userId);
        void AddUpdateCommentAndRating(string comment, int rating, Guid id);
        new Order Create(Order order);
    }
}