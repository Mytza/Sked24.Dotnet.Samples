using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Sked24.Dotnet.Samples.OdataClient.WebApiModel.DTO;
using Sked24.Dotnet.Samples.OdataClient.WebApiModel.Enum;

namespace Sked24.Dotnet.Samples.MVC.Models
{
    public class Appointment 
    {
        public Appointment()
        {
            StatusHistories = new List<AppointmentStatusHistory>();
        }
        public bool Cancelled { get; set; }
        public Guid Id { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public String RowVersion { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedByClient { get; set; }
        public string CreatedByClient { get; set; }
        public DateTimeOffset DateTimeFrom { get; set; }

        public DateTimeOffset DateTimeTo { get; set; }

        public DateTimeOffset DateTimeFromUTC { get; set; }

        public DateTimeOffset DateTimeToUTC { get; set; }

        public AppointmentStatus Status { get; set; }
        public int Duration { get; set; }

        public Guid PhysicianId { get; set; }
        public PhysicianDTO Physician { get; set; }
        public Guid? SpecialityId { get; set; }
        public SpecialityDTO Speciality { get; set; }
        public Guid ServiceId { get; set; }
        public ServiceDTO Service { get; set; }
        public Guid? InsuranceId { get; set; }
        public InsuranceDTO Insurance { get; set; }
        public Guid PatientId { get; set; }
        public PatientDTO Patient { get; set; }
        public Guid CenterId { get; set; }
        public CenterDTO Center { get; set; }
        public bool IsOvercapacity { get; set; }
        public Guid? InCallBy { get; set; }
        public int Index { get; set; }
        public int? Number { get; set; }
        public int NumberOfSlots { get; set; }
        public ICollection<AppointmentStatusHistory> StatusHistories { get; set; }
    }
}