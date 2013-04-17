using System.Web.Mvc;
using System.Web.Security;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Models;

namespace PersonalServiceBus.RSS.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthentication _authentication;

        public AccountController(IAuthentication authentication)
        {
            _authentication = authentication;
        }

        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var userValidationResponse = _authentication.ValidateUser(new User(model.UserName, model.Password));

                if (userValidationResponse.Status.ErrorLevel == ErrorLevel.Error)
                {
                    ModelState.AddModelError("", userValidationResponse.Status.ErrorMessage);
                }
                else if (userValidationResponse.Status.ErrorLevel == ErrorLevel.Critical)
                {
                    ModelState.AddModelError("", string.Format("Error logging in {0}", 
                        userValidationResponse.Status.ErrorMessage));
                }
                else
                {
                    if (userValidationResponse.Data)
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            return View(model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                var user = new User(model.UserName, model.Password, model.Email);
                var status = _authentication.Register(user);

                if (status.ErrorLevel < ErrorLevel.Error)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", status.ErrorMessage);
            }

            return View(model);
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var passwordChangeResponse = _authentication.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);

                if (passwordChangeResponse.Status.ErrorLevel == ErrorLevel.Error)
                {
                    ModelState.AddModelError("", passwordChangeResponse.Status.ErrorMessage);
                }
                else if (passwordChangeResponse.Status.ErrorLevel == ErrorLevel.Critical)
                {
                    ModelState.AddModelError("", string.Format("Error changing password {0}",
                        passwordChangeResponse.Status.ErrorMessage));
                }
                else
                {
                    if (passwordChangeResponse.Data)
                    {
                        return RedirectToAction("ChangePasswordSuccess");
                    }
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            return View(model);
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }
    }
}
