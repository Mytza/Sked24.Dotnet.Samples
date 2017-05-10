﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Ajax.Utilities;
using Microsoft.OData.Client;
using Sked24.Dotnet.Samples.MVC.Models;
using Sked24.Dotnet.Samples.OdataClient.EntitySets;
using Sked24.Dotnet.Samples.OdataClient.WebApiModel.DTO;
using Sked24.Dotnet.Samples.OdataClient.WebApiModel.Enum;

namespace Sked24.Dotnet.Samples.MVC.Business
{
    public class ReadResourceData
    {
        private const int PackageSize = 100;
        public static async Task<IEnumerable<ResourceAppointmentsDto>> GetResourceData(Container container, ResourceReportFiltersDto filters)
        {
            var appointmentsQuery =
                container.Appointments.IncludeTotalCount().Where(
                    x =>
                        x.Cancelled == false && x.DateTimeFromUTC >= filters.DateFrom.ToUniversalTime() &&
                        x.DateTimeToUTC <= filters.DateTo.ToUniversalTime() && x.PhysicianId == filters.ResourceId);
            var countQuery = appointmentsQuery.Take(0);

            var countRequest = countQuery as DataServiceQuery<AppointmentDTO>;

            //get number of appointments
            var countResult = await countRequest.ExecuteAsync();
            var count = ((QueryOperationResponse) countResult).TotalCount;

            //duplicate for cancelled appoitnmetns
            var cancelledAppointmentsQuery =
                container.Appointments.IncludeTotalCount().Where(
                    x =>
                        x.Cancelled == false && x.DateTimeFromUTC >= filters.DateFrom.ToUniversalTime() &&
                        x.DateTimeToUTC <= filters.DateTo.ToUniversalTime() && x.PhysicianId == filters.ResourceId);
            var cancelledCountQuery = cancelledAppointmentsQuery.Take(0);

            var cancelledCountRequest = cancelledCountQuery as DataServiceQuery<AppointmentDTO>;

            //get number of appointments
            var cancelCountResult = await cancelledCountRequest.ExecuteAsync();
            var cancelledCount = ((QueryOperationResponse)cancelCountResult).TotalCount;

            var totalCount = cancelledCount + count;

            if (totalCount > 0)
            {

                //get physician
                var resource = await container.Physicians.ByKey(filters.ResourceId).GetValueAsync();
                //we have results we proceed on taking pages, we will set the page to package size
                var offset = 0;
                var tasks = new List<Task<IEnumerable<AppointmentDTO>>>();
                while (offset < count)
                {
                    tasks.Add(GetAppointments(offset,PackageSize, container, filters));
                    offset += PackageSize;
                }
                var results = await Task.WhenAll(tasks);
                var appointments = results.SelectMany(x => x).ToList();

                //get cancelled appointments
                offset = 0;
                tasks = new List<Task<IEnumerable<AppointmentDTO>>>();
                while (offset < cancelledCount)
                {
                    tasks.Add(GetCancelledAppointments(offset, PackageSize, container, filters));
                    offset += PackageSize;
                }
                var cancelledResults = await Task.WhenAll(tasks);
                appointments.AddRange(cancelledResults.SelectMany(x =>x));

                //we now have read all appointments we need to read user,center and service information
                var centerIds = appointments.Select(x => x.CenterId).Distinct();

                var centerQuery = container.Centers.Where(x => centerIds.Contains(x.Id) && x.Cancelled == false);
                var centerRequest = centerQuery as DataServiceQuery<CenterDTO>;
                var centerQueryResult = await centerRequest.ExecuteAsync();
                var centers = centerQueryResult.ToList();

                var serviceIds = appointments.Select(x => x.ServiceId).Distinct();

                var serviceQuery = container.Services.Expand(x => x.Speciality).Where(x => serviceIds.Contains(x.Id) && x.Cancelled == false);
                var serviceRequest = serviceQuery as DataServiceQuery<ServiceDTO>;
                var serviceQueryResult = await serviceRequest.ExecuteAsync();
                var services = serviceQueryResult.ToList();


                //CreatedBy user
                var userIds = appointments.Select(x => x.CreatedBy);
                //parse to guid
                var usersParsed = new List<Guid>();
                foreach (var user in userIds)
                {
                    Guid guid;
                    if (Guid.TryParse(user, out guid))
                    {
                        usersParsed.Add(guid);
                    }
                }

                //Append users who cancelled
                var cancelledUsers = appointments.Where(x => x.Cancelled).Select(x => x.ModifiedBy);
                foreach (var cancelledUser in cancelledUsers)
                {
                    Guid guid;
                    if (Guid.TryParse(cancelledUser, out guid))
                    {
                        usersParsed.Add(guid);
                    }
                }
                usersParsed = usersParsed.Distinct().ToList();

                //append users who confimed
                var usersWhoConfirmed =
                    appointments.SelectMany(
                        x =>
                            x.StatusHistories.Where(y => y.Status == AppointmentStatus.Confirmed)
                                .Select(y => y.CreatedBy));
                foreach (var user in usersWhoConfirmed)
                {
                    Guid guid;
                    if (Guid.TryParse(user, out guid))
                    {
                        usersParsed.Add(guid);
                    }
                }

                var userQuery = container.Users.Where(x => usersParsed.Contains(x.Id));
                var userRequest = userQuery as DataServiceQuery<UserDTO>;
                var userResult = await userRequest.ExecuteAsync();
                var users = userResult.ToList();

                //convert appointments to required 
                var result = appointments.Select(x => new ResourceAppointmentsDto
                {
                    Id =  x.Id,
                    DocumentNumber = x.Patient.DocumentNumber,
                    ClientName = x.Patient.FullName,
                    AppointmentDate = x.DateTimeFrom.Date.ToShortDateString(),
                    AppointmentTime = x.DateTimeFrom.TimeOfDay.ToString(),
                    Status = x.Status.ToString(),
                    CreatedOn = x.CreatedOn,
                    CreatedBy = users.FirstOrDefault(u => u.Id.ToString() == x.CreatedBy)?.Username ??  x.CreatedBy,
                    Confirmed = x.StatusHistories.FirstOrDefault(s => s.Status == AppointmentStatus.Confirmed) != null,
                    ConfirmedOn = x.StatusHistories.FirstOrDefault(s => s.Status == AppointmentStatus.Confirmed)?.CreatedOn,
                    ConfirmedBy = x.StatusHistories.FirstOrDefault(s => s.Status == AppointmentStatus.Confirmed)?.CreatedBy,
                    Cancelled = x.Cancelled,
                    CancelledBy = !x.Cancelled ? string.Empty :users.FirstOrDefault(u => u.Id.ToString() == x.ModifiedBy)?.Username ?? x.ModifiedBy,
                    CancelledOn = !x.Cancelled ? null : (DateTimeOffset?)x.ModifiedOn,
                    IsOvercapacity = x.IsOvercapacity,
                    Area = services.FirstOrDefault(s => s.Id == x.ServiceId)?.Area.ToString(),
                    ResourceDocumentNumber = resource.DocumentNumber,
                    ResourceName = resource.FullName,
                    ServiceName = services.FirstOrDefault(s => s.Id == x.ServiceId)?.Name,
                    SpecialityName = services.FirstOrDefault(s => s.Id == x.ServiceId)?.Speciality.Name,
                    LocationName = centers.FirstOrDefault(c => c.Id == x.CenterId)?.Name,
                    CreatedByChannel = x.CreatedByClient
                });
                return result;
            }
            return new List<ResourceAppointmentsDto>();
        }

        public static async Task<IEnumerable<AppointmentDTO>> GetAppointments(int offset, int packageSize, Container container, ResourceReportFiltersDto filters)
        {
            var query = container.Appointments.Expand(x => x.Patient).Expand(x => x.StatusHistories).Where(
                x =>
                    x.Cancelled == false && x.DateTimeFromUTC >= filters.DateFrom.ToUniversalTime() &&
                    x.DateTimeToUTC <= filters.DateTo.ToUniversalTime() && x.PhysicianId == filters.ResourceId);
            var appointmentQuery = query.Skip(offset).Take(packageSize);
            var appointmentRequest = appointmentQuery as DataServiceQuery<AppointmentDTO>;

            var result = await appointmentRequest.ExecuteAsync();
            var resultSet = result.Select(x => x).ToList();
            return resultSet;
        }

        public static async Task<IEnumerable<AppointmentDTO>> GetCancelledAppointments(int offset, int packageSize, Container container, ResourceReportFiltersDto filters)
        {
            var query = container.Appointments.Expand(x => x.Patient).Expand(x => x.StatusHistories).Where(
                x =>
                    x.Cancelled && x.DateTimeFromUTC >= filters.DateFrom.ToUniversalTime() &&
                    x.DateTimeToUTC <= filters.DateTo.ToUniversalTime() && x.PhysicianId == filters.ResourceId);
            var appointmentQuery = query.Skip(offset).Take(packageSize);
            var appointmentRequest = appointmentQuery as DataServiceQuery<AppointmentDTO>;

            var result = await appointmentRequest.ExecuteAsync();
            var resultSet = result.ToList<AppointmentDTO>();
            return resultSet;
        }
    }
}