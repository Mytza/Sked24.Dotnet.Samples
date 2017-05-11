using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sked24.Dotnet.Samples.OdataClient.WebApiModel.DTO;
using Sked24.Dotnet.Samples.OdataClient.WebApiModel.Enum;

namespace Sked24.Dotnet.Samples.MVC.Models
{
    public class AppointmentStatusHistory
    {
        public bool Cancelled { get; set; }
        public Guid Id { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public String RowVersion { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedByClient { get; set; }
        public string CreatedByClient { get; set; }
        public AppointmentStatus Status { get; set; }
        public string TransitionReasonOthers { get; set; }
        public Guid? AppointmentStatusTransitionReasonId { get; set; }
    }
}