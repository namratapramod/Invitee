using AutoMapper;
using Invitee.Entity;
using Invitee.Repository.Infra;
using Invitee.Utils;
using Invitee.ViewModels;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;

namespace Invitee.Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private RepositoryContext repositoryContext;
        public UserRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            this.repositoryContext = repositoryContext;
        }

        public User RegisterUser(UserViewModel userModel)
        {
            User user = Mapper.Map<User>(userModel);
            var result = userModel.UserName.Substring(0, 3);
            user.ReferralCodeToBeShared = (result + General.GetRandomString()).ToLower();
            return repositoryContext.Users.Add(user);
        }
        public User ValidateUser(string userName, string password)
        {
            var user = RepositoryContext.Users.Where(x => x.UserName == userName || x.Mobile == userName || x.Email == userName).SingleOrDefault();
            if (user == null)
                return null;
            repositoryContext.Entry<User>(user).Reload();
            return Encryption.CompareHash(user.Password, password) ? user : null;
        }

        public User ValidateAdminUser(string userName, string password)
        {
            //var compareHash = Encryption.CompareHash(x.Password, password);
            return repositoryContext.Users.Where(x => x.UserName == userName && x.UserRole == Entity.Enums.UserRole.Admin).AsEnumerable().Where(x => Encryption.CompareHash(x.Password, password)).FirstOrDefault();
        } 

        public User EmailExists(string email)
        {
            return repositoryContext.Users.Where(x => x.Email == email).SingleOrDefault();
        }
        public bool UpdatePassword(User user)
        {
            try
            {
                repositoryContext.Entry(user).State = System.Data.Entity.EntityState.Modified;
                repositoryContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int GetNumberOfReferralUsed(string referralCode)
        {
            return repositoryContext.Users.Where(x => x.ReferralCodeApplied == referralCode).Count();
        }
        public bool ChangePassword(int userId, ChangePasswordModel changePasswordModel)
        {
            try
            {
                var user = repositoryContext.Users.Find(userId);
                if(!Encryption.CompareHash(user.Password, changePasswordModel.OldPassword))
                {
                    return false;
                }
                user.Password = Encryption.HashString(changePasswordModel.NewPassword);
                repositoryContext.Entry(user).State = System.Data.Entity.EntityState.Modified;
                repositoryContext.SaveChanges();
                return true;
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}