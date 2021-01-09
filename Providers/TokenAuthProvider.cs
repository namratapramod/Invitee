using Invitee.Entity;
using Invitee.Providers.Google;
using Invitee.Repository;
using Invitee.ViewModels;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Invitee.Providers
{
    public class TokenAuthProvider : OAuthAuthorizationServerProvider
    {
        private User user = null;
        private IRepositoryWrapper _repoWrapper;
        private GoogleTokenVerifier _googleTokenVerifier;
        public TokenAuthProvider()
        {
            _repoWrapper = new RepositoryWrapper(new RepositoryContext());
            _googleTokenVerifier = new GoogleTokenVerifier();
        }
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            context.AdditionalResponseParameters.Add("UserId", this.user.Id);
            context.AdditionalResponseParameters.Add("Firstname", this.user.FirstName);
            context.AdditionalResponseParameters.Add("Lastname", this.user.LastName);
            context.AdditionalResponseParameters.Add("Username", this.user.UserName);
            context.AdditionalResponseParameters.Add("Email", this.user.Email);
            context.AdditionalResponseParameters.Add("ReferralCodeToBeShared", this.user.ReferralCodeToBeShared);
            context.AdditionalResponseParameters.Add("ReferralCountUsed", this._repoWrapper.User.GetNumberOfReferralUsed(this.user.ReferralCodeToBeShared));
            context.AdditionalResponseParameters.Add("EarnedVideo", this.user.EarnedVideo);
            context.AdditionalResponseParameters.Add("ClaimedVideo", this.user.ClaimedVideo);
            context.AdditionalResponseParameters.Add("Mobile", this.user.Mobile);
            return base.TokenEndpoint(context);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var currentReq = HttpContext.Current.Request;

            if (!string.IsNullOrEmpty(currentReq.Form["google_token"]))
            {
                var res = _googleTokenVerifier.ValidateGoogleToken(currentReq.Form["google_token"]);
                if (res == null){
                    context.SetError("invalid_gtoken", "Invalid google token.");
                    return;
                }
                else
                {
                    var mobileNumber = currentReq.Form["mobile"].Trim() ?? "";
                    var isValid = Regex.Match(mobileNumber, "(0/91)?[1-9][0-9]{9}").Success;
                    if (!isValid)
                    {
                        context.SetError("invalid_request", "A valid Mobile numnber is required.");
                        return;
                    }
                    var existingUser = _repoWrapper.User.FindByCondition(x => x.Email.ToLower().Trim() == res.Email.ToLower().Trim()).SingleOrDefault();
                    if (existingUser == null)
                    {
                        var dummyPass = new Random().Next(1000, 100000).ToString();
                        var userViewModel = new UserViewModel
                        {
                            EarnedVideo = _repoWrapper.Config.FindAll().Select(x => x.FreeVideoOnRegister).FirstOrDefault(),
                            Email = res.Email,
                            FirstName = res.FirstName,
                            LastName = res.LastName,
                            ProfilePic = res.ProfilePic,
                            UserRole = Entity.Enums.UserRole.Normal,
                            UserName = res.Email.Split('@')[0],
                            IsGoogleSignIn = true,
                            Mobile = mobileNumber,
                            Password = dummyPass,
                            ConfirmPassword = dummyPass
                        };
                        try
                        {
                            var regResponse = _repoWrapper.User.RegisterUser(userViewModel);
                            this._repoWrapper.Save();
                            this.user = regResponse;
                        }
                        catch (Exception ex)
                        {
                            string errMsg = ex.Message;
                            if (ex.InnerException != null)
                            {
                                errMsg = ex.InnerException.Message;
                                if (ex.InnerException.InnerException != null)
                                    errMsg = ex.InnerException.InnerException.Message;
                            }
                            context.SetError(errMsg);
                            return;
                        }
                        
                    }
                    else
                    {
                        this.user = existingUser;
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(context.UserName) || string.IsNullOrEmpty(context.Password))
                {
                    context.SetError("invalid_grant", "Username or password is missing");
                    return;
                }

                var user = _repoWrapper.User.ValidateUser(context.UserName, context.Password);
                if (user == null)
                {
                    context.SetError("invalid_grant", "Provided username and password is incorrect");
                    return;
                }
                this.user = user;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Role, user.UserRole.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim("Mobile", user.Mobile));
            identity.AddClaim(new Claim("Id", user.Id.ToString()));
            identity.AddClaim(new Claim("DeviceId", user.DeviceId ?? ""));
            identity.AddClaim(new Claim("Email", user.Email));
            context.Validated(identity);
        }
    }
}