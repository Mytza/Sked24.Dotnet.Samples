using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sked24.Dotnet.Samples.MVC.Models
{
    public class ResourceReportFiltersDto
    {
        public DateTimeOffset DateFrom { get; set; }
        public DateTimeOffset DateTo { get; set; }
        public string ResourceName { get; set; }
        public Guid ResourceId { get; set; }
    }
}