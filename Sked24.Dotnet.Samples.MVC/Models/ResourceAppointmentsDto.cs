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
        public DateTimeOffset AppointmentDate { get; set; }
        public AppointmentStatus Status { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public bool Confirmed { get; set; }
        public DateTimeOffset? ConfirmedOn { get; set; }
        public string ConfirmedBy { get; set; }
        public DateTimeOffset? CancelledOn { get; set; }
        public string CancelledBy { get; set; }
        public bool IsOvercapacity { get; set; }
        public DateTimeOffset? OvercapacityDate { get; set; }
        public string OvercapacityUser { get; set; }
        public MedicalArea Area { get; set; }
        public string ResourceDocumentNumber { get; set; }
        public string ResourceName { get; set; }
        public string ServiceName { get; set; }
        public string LocationName { get; set; }
        public string CreatedByChannel { get; set; }
    }
}