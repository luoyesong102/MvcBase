using System;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Esmart.Permission.Application.Services;
using Esmart.Permission.Web.Models.Permissions;
using Esmart.Framework.Logging;
using Esmart.Framework.Model;
using Esmart.Permission.Application;
namespace Esmart.Permission.Web.Controllers
{
    public class AccountController : BaseController
    {
        readonly UserService _userService;

        public AccountController()
        {
            _userService = new UserService();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string UserName, string Password, string url)
        {
            try
            {
             
                var authenService = new AuthenticationService();

                var user = authenService.GetUser((UserName ?? "").Trim(), Password);

                var claimsId = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name,user.TrueName??user.Ename??""),
                    new Claim(ClaimTypes.NameIdentifier,user.UserID.ToString())
                }, DefaultAuthenticationTypes.ApplicationCookie);
                Request.GetOwinContext().Authentication.SignIn(claimsId);

                if (string.IsNullOrEmpty(url))
                {
                    return RedirectToAction("Index", "Home");
                }
               url+= "?Token=" + Uri.EscapeDataString(CommonFunction.Encrypt(user.UserID.ToString()));
                return Redirect(url);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = ex.Message;
                Esmart.Framework.Logging.LogManager.CreateLog4net().Error("applicationError", ex);
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                int userId;
                token = Uri.UnescapeDataString(token);
                token = CommonFunction.Decrypt(token);
                if (int.TryParse(token, out userId))
                {
                    var user = new UserManager().GetSingleUser(userId);
                    if (user != null)
                    {
                        var claimsId = new ClaimsIdentity(new[]{
                            new Claim(ClaimTypes.Name,user.TrueName??user.Ename??""),
                            new Claim(ClaimTypes.NameIdentifier,user.UserID.ToString()) }, DefaultAuthenticationTypes.ApplicationCookie);
                        Request.GetOwinContext().Authentication.SignIn(claimsId);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View();
        }

        public ActionResult LogOff()
        {
            Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ModifyPassword(int uid = 0)
        {
            string account = null;
            var user = _userService.GetUser(uid);
            if (user != null)
            {
                account = user.Email;
            }
            return View(new ModifyUserPassword { UserAccount = account });
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ModifyPassword(ModifyUserPassword model, string url)
        {
            try
            {
                _userService.ModifyPassword(model.UserAccount, model.Password, model.NewPassword);
            }
            catch (Exception ex)
            {
                ViewBag.message = ex.Message;
                return View(model);
            }
            return Login(model.UserAccount, model.NewPassword, url);
        }
    }
}
