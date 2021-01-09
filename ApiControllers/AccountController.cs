using Invitee.Entity.Enums;
using Invitee.Infrastructure;
using Invitee.Repository;
using Invitee.Utils;
using Invitee.ViewModels;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using Invitee.Utils;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Invitee.Models;
using System.Web.Security;

namespace Invitee.ApiControllers
{
    [RoutePrefix("account")]
    public class AccountController : BaseApiController
    {

        private readonly IRepositoryWrapper repositoryWrapper;
        private readonly IUserRepository userRepository;
        private readonly ILogger logger;

        public AccountController(IRepositoryWrapper repositoryWrapper, ILogger logger, IUserRepository userRepository)
        {
            this.repositoryWrapper = repositoryWrapper;
            this.userRepository = userRepository;
            this.logger = logger.ForContext(this.GetType());
        }

        [HttpPost]
        public async Task<IHttpActionResult> Register(UserViewModel user)
        {
            logger.Information("Register called {@user}", user);
            try
            {
                if (ModelState.IsValid)
                {
                    user.UserRole = UserRole.Normal; //Make sure the userType is not set to admin.
                    user.EarnedVideo = repositoryWrapper.Config.FindAll().Select(x => x.FreeVideoOnRegister).FirstOrDefault();
                    if (!string.IsNullOrEmpty(user.ReferralCodeApplied))
                    {
                        var referrealUserId = repositoryWrapper.User.FindAll(true).Where(x => x.ReferralCodeToBeShared == user.ReferralCodeApplied).SingleOrDefault();
                        if (referrealUserId == null)
                        {
                            ModelState.AddModelError("Error", "Invalid Referral code used\nPlease verify the referral code");
                            return ModelError();
                        }
                        else
                        {
                            var referralSetting = repositoryWrapper.Config.GetLatestReferralSetting();
                            if (referralSetting != null)
                            {
                                if ((referrealUserId.ReferralIncrementer + 1) >= referralSetting.ReferralCountForFreeVideo)
                                {
                                    referrealUserId.ReferralIncrementer = 0;
                                    referrealUserId.EarnedVideo += 1;
                                }
                                else
                                {
                                    referrealUserId.ReferralIncrementer += 1;
                                }
                            }

                        }

                    }
                    var result = repositoryWrapper.User.RegisterUser(user);
                    repositoryWrapper.Save();
                    var base_url = string.Format("{0}://{1}{2}", Request.RequestUri.Scheme, Request.RequestUri.Authority, Url.Content("~"));
                    var welcomeEmail = Properties.Resources.WelcomeEmail.Replace("[name]", result.UserName).Replace("[referral_code]", result.ReferralCodeToBeShared)
                        .Replace("[welcome_img]", "http://invitee-stage.balendin.in/WebCore/dist/img/illo_welcome_1.png");
                    await Mailer.SendEmail(user.Email, "Welcome to Invitee", welcomeEmail);
                    if (result != null)
                        return Success(result);
                    else
                        return InternalServerError();
                }
            }
            catch (Exception ex)
            {
                var message = ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message;
                ModelState.AddModelError("Error", message);
                await Mailer.SendEmail("loquininvitee@gmail.com", "Register error", message);
            }
            return ModelError();
        }

        [Authorize(Roles = "Admin")]
        public IHttpActionResult Get()
        {
            return Success(repositoryWrapper.User.FindAll().ToList());
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Normal")]
        public IHttpActionResult GetProfile()
        {
            var userId = GetUserId();
            var userInfo = repositoryWrapper.User.FindByCondition(x => x.Id == userId).Single();
            var result = new
            {
                UserInfo = userInfo,
                ReferralInfo = repositoryWrapper.User.FindByCondition(x => x.ReferralCodeApplied == userInfo.ReferralCodeToBeShared).Select(x => new {
                    x.UserName,
                    FullName = x.FirstName + " " + x.LastName,
                    x.ProfilePic
                }),
                ReferralSettingNumber = repositoryWrapper.Config.GetLatestReferralSetting().ReferralCountForFreeVideo
            };
            return Success(result);
        }

        [HttpPost]
        [ApiAuthorize]
        public IHttpActionResult Updateprofile()
        {
            logger.Information("Update profile called by " + GetUserName());
            try
            {
                var request = HttpContext.Current.Request;
                var userId = GetUserId();
                var user = repositoryWrapper.User.FindByCondition(x => x.Id == userId).Single();
                var freeCoinstoBeApplied = 0;
                int? referralUserId = 0;
                if (request.Files.Count > 0)
                {
                    HttpPostedFileBase file = new HttpPostedFileWrapper(request.Files["profilePic"]);
                    file.ValidateImageFile();

                    var newFilenName = file.GetNewFileName();
                    var profilePicPath = HttpContext.Current.Server.MapPath($"~/{AppSettings.ProfilePicturesPath}");
                    if (!Directory.Exists(profilePicPath))
                        Directory.CreateDirectory(profilePicPath);

                    var fullFilePath = Path.Combine(profilePicPath, newFilenName);
                    file.SaveAs(fullFilePath);
                    user.ProfilePic = $"{AppSettings.ProfilePicturesPath}{newFilenName}";

                }
                if (!string.IsNullOrEmpty(request.Form["referralCodeToBeApplied"]))
                {
                    if (!string.IsNullOrEmpty(user.ReferralCodeApplied))
                    {
                        ModelState.AddModelError("Error", "Referral code has been already applied");
                    }
                    else
                    {
                        string refferalCodeString = request.Form["referralCodeToBeApplied"];
                        var referrealUserId = repositoryWrapper.User.FindAll(true).Where(x => x.ReferralCodeToBeShared == refferalCodeString).SingleOrDefault();
                        if (referrealUserId == null)
                        {
                            ModelState.AddModelError("Error", "Invalid Referral code used\nPlease verify the referral code");
                            return ModelError();
                        }
                        else
                        {
                            var referralSetting = repositoryWrapper.Config.GetLatestReferralSetting();
                            if (referralSetting != null)
                            {
                                if ((referrealUserId.ReferralIncrementer + 1) >= referralSetting.ReferralCountForFreeVideo)
                                {
                                    referrealUserId.ReferralIncrementer = 0;
                                    referrealUserId.EarnedVideo += 1;
                                }
                                else
                                {
                                    referrealUserId.ReferralIncrementer += 1;
                                }
                            }
                            user.ReferralCodeApplied = request.Form["referralCodeToBeApplied"];
                        }
                    }

                }
                if (!string.IsNullOrEmpty(request.Form["email"]))
                {
                    try
                    {
                        var address = new MailAddress(request.Form["email"]);
                        user.Email = request.Form["email"];
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("Error", ex.Message);
                    }

                }
                if (!string.IsNullOrEmpty(request.Form["username"]))
                {
                    if (user.UserName.Length < 3)
                    {
                        ModelState.AddModelError("Error", "Invalid length for username");
                    }
                    else
                    {
                        user.UserName = request.Form["username"];
                    }
                }
                if (!string.IsNullOrEmpty(request.Form["firstname"]))
                {
                    user.FirstName = request.Form["firstname"];
                }
                if (!string.IsNullOrEmpty(request.Form["lastname"]))
                {
                    user.LastName = request.Form["lastname"];
                }
                if (!string.IsNullOrEmpty(request.Form["mobile"]))
                {
                    var isValid = Regex.Match(request.Form["mobile"], "(0/91)?[1-9][0-9]{9}").Success;
                    if (!isValid)
                    {
                        ModelState.AddModelError("Error", request.Form["mobile"]);
                    }
                    else
                    {
                        user.Mobile = request.Form["mobile"];
                    }
                }
                if (!string.IsNullOrEmpty(request.Form["ReferralCodeToBeShared"]))
                {
                    user.ReferralCodeToBeShared = request.Form["ReferralCodeToBeShared"];
                }
                if (!string.IsNullOrEmpty(request.Form["dateofbirth"]))
                {
                    user.DateOfBirth = DateTime.Parse(request.Form["dateofbirth"]);
                }
                if (!string.IsNullOrEmpty(request.Form["gender"]))
                {
                    user.Gender = (Gender)Enum.Parse(typeof(Gender), request.Form["gender"]);
                }

                if (!string.IsNullOrEmpty(request.Form["deviceId"]))
                {
                    user.DeviceId = request.Form["deviceId"];
                }
                if (!string.IsNullOrEmpty(request.Form["country"]))
                {
                    user.Country = request.Form["country"];
                }
                if (!ModelState.IsValid)
                {
                    return ModelError();
                }

                repositoryWrapper.User.Update(user);
                repositoryWrapper.Save();
                return Success(true);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error on Update profile");
                ModelState.AddModelError("Error", ex.Message);
                return ModelError();
            }
        }

        [HttpGet]
        [ApiAuthorize]
        public IHttpActionResult GetEarnedVideoCount()
        {
            var userId = GetUserId();
            var userInfo = repositoryWrapper.User.FindByCondition(x => x.Id == userId).Select(x => new {
                x.EarnedVideo,
                x.ClaimedVideo
            });
            return Success(userInfo);
        }
        [HttpPost]
        public IHttpActionResult ChangePassword(ChangePasswordModel changePasswordModel)
        {
            logger.Information("ChangePassword action called", changePasswordModel);
            var userId = GetUserId();
            if (ModelState.IsValid)
            {
                var data = repositoryWrapper.User.FindByCondition(x => x.Id == userId).SingleOrDefault();
                if(data != null)
                {
                    if(data.IsGoogleSignIn == true)
                    {
                        return Error("Cannot process the request for google users");
                    }
                    else
                    {
                        var result = userRepository.ChangePassword(userId, changePasswordModel);
                        return result ? Success(result) : ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Invalid Old Password"));
                    }
                }
                return NotFound();
            }
            else
            {
                return ModelError();
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> ForgetPassword(UserLoginModel userLoginModel)
        {
            logger.Information("ForgetPassword action called", userLoginModel);
            if (ModelState.IsValid)
            {
                var user = userRepository.EmailExists(userLoginModel.Email);
                if (user != null)
                {
                    if (user.IsGoogleSignIn == true)
                    {
                        return Error("Cannot process the request for google users");
                    }
                    else
                    {
                        var newpass = Membership.GeneratePassword(8, 4);
                        user.Password = Encryption.HashString(newpass);
                        userRepository.UpdatePassword(user);
                        var passEmail = Properties.Resources.ForgetEmail.Replace("[new_password]", newpass);
                        var result = await Mailer.SendEmail(userLoginModel.Email, "Your reset password", passEmail);
                        return result ? Success(result) : ResponseMessage(new HttpResponseMessage(HttpStatusCode.InternalServerError));
                    }
                }
                return NotFound();
            }
            return ModelError();
        }
    }
}
