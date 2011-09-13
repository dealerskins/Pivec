using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Pivec.Promotion.Web.Models;

namespace Pivec.Promotion.Web.Controllers
{
    public class AccountController : Controller
    {
        /// <summary>
        /// Displays the LogOn page.
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOn()
        {
            return View();
        }

        /// <summary>
        /// Validates the LogOn information.
        /// </summary>
        /// <param name="model">LogOn model to validate.</param>
        /// <param name="returnUrl">Url to redirect to.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                    if (Roles.IsUserInRole(model.UserName, "Administrator"))
                    {
                        return RedirectToAction("Index", "Customer");
                    }
                    else
                    {
                        return Redirect("/Rules");   
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }
            
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Log Off the current user.
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction(string.Empty, string.Empty);
        }
    }
}
