using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.OData.Client;
using Sked24.Dotnet.Samples.MVC.Business;
using Sked24.Dotnet.Samples.MVC.Models;
using Sked24.Dotnet.Samples.OdataClient.EntitySets;
using Sked24.Dotnet.Samples.OdataClient.WebApiModel.DTO;

namespace Sked24.Dotnet.Samples.MVC.Controllers
{
    public class ResourceReportController : Controller
    {
        private const string ThirdPartyApiHeader = "ThirdApiKey";
        private const string UriAppSetting = "Sked24Uri";
        private const string ThirdApiKeyAppSetting = "ThirdApiKey";

        private Container container;
        private readonly string thirdPartyKey;
        public ResourceReportController()
        {
            //read from config
            var uri = ConfigurationManager.AppSettings[UriAppSetting];
            thirdPartyKey = ConfigurationManager.AppSettings[ThirdApiKeyAppSetting];
            
            container = new Container(new Uri(uri));
            //add header for each outgong request
            container.SendingRequest2 += ContainerOnSendingRequest2;
            container.EntityParameterSendOption = EntityParameterSendOption.SendOnlySetProperties;
            container.MergeOption = MergeOption.OverwriteChanges;
        }

        private void ContainerOnSendingRequest2(object sender, SendingRequest2EventArgs sendingRequest2EventArgs)
        {
            sendingRequest2EventArgs.RequestMessage.SetHeader(ThirdPartyApiHeader, thirdPartyKey);
        }

        // GET: ResourceReport
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(ResourceReportFiltersDto filters)
        {
            var result = await ReadResourceData.GetResourceData(container, filters);
            var data = new byte[0];
            return File(data, "application/vnd.ms-excel", "mytestfile.xls");
        }

        [HttpGet]
        public async Task<ActionResult> ResourceAutocomplete(string prefix)
        {
            var query =
                container.Physicians.
                    Where(x => x.Cancelled == false && x.FullName.StartsWith(prefix))
                    .OrderBy(x => x.FullName)
                    .Take(50);
                    //.Select(x => new
                    //{
                    //    Id = x.Id,
                    //    FullName = x.FullName
                    //});

            var request = query as DataServiceQuery<PhysicianDTO>;

            var response = await request.ExecuteAsync();
            var a = response.ToList();
           // var collection = new DataServiceCollection<PhysicianDTO>(response, TrackingMode.AutoChangeTracking, "Physicians", null, null);
            var result = a.Select(x => new {Id = x.Id, Name = x.FullName});
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}