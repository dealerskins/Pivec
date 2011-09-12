using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pivec.Promotion.Web.Controllers
{
    public class RulesController : Controller
    {

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

    }
}
