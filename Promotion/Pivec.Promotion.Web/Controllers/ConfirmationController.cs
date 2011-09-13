using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Pivec.Promotion.Web.Controllers
{
    public class ConfirmationController : Controller
    {
        //
        // GET: /Confirmation/

        public ActionResult Index()
        {
            FormsAuthentication.SignOut();
            return View();
        }

    }
}
