using Invitee.Entity;
using Invitee.Repository.Infra;
using Invitee.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invitee.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        User ValidateUser(string userName, string password);
        User ValidateAdminUser(string userName, string password);
        User RegisterUser(UserViewModel userModel);
        User EmailExists(string email);
        bool UpdatePassword(User user);
        int GetNumberOfReferralUsed(string referralCode);
        bool ChangePassword(int userId, ChangePasswordModel changePasswordModel);
    }
}