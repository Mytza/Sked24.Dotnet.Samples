using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sked24.Dotnet.Samples.MVC.Models;

namespace Sked24.Dotnet.Samples.MVC.Controllers
{
    public class ResourceReportController : Controller
    {
        // GET: ResourceReport
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ResourceReportFiltersDto filters)
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResourceAutocomplete(string Prefix)
        {
            return View();
        }
    }
}