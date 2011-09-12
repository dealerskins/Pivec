using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pivec.Promotion.Web.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Error(string errorMessage = "")
        {
            ViewData.Add(new KeyValuePair<string, object>("ErrorMessage", errorMessage)) ;
            return View();
        }

    }
}
