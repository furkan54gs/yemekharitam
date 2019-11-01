using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcCF5.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult NotFound()
        {
            return View();
        }
        public ActionResult NotAuthorized()
        {
            return View();
        }
        public ActionResult ErrorPage()
        {
            return View();
        }
    }
}