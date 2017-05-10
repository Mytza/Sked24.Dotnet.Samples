using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sked24.Dotnet.Samples.OdataClient.WebApiModel.Enum;

namespace Sked24.Dotnet.Samples.MVC.Models
{
    public class ResourceAppointmentsDto
    {
        public Guid Id { get; set; }
        public string DocumentNumber { get; set; }
        public string ClientName { get; set; }
        public string AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }
        public string Status { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public bool Confirmed { get; set; }
        public DateTimeOffset? ConfirmedOn { get; set; }
        public string ConfirmedBy { get; set; }
        public bool Cancelled { get; set; }
        public DateTimeOffset? CancelledOn { get; set; }
        public string CancelledBy { get; set; }
        public bool IsOvercapacity { get; set; }
        public string Area { get; set; }
        public string ResourceDocumentNumber { get; set; }
        public string ResourceName { get; set; }
        public string ServiceName { get; set; }
        public string SpecialityName { get; set; }
        public string LocationName { get; set; }
        public string CreatedByChannel { get; set; }
    }
}