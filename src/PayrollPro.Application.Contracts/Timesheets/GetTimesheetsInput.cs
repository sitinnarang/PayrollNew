using System;
using Volo.Abp.Application.Dtos;

namespace PayrollPro.Timesheets
{
    public class GetTimesheetsInput : PagedAndSortedResultRequestDto
    {
        public Guid? EmployeeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimesheetStatus? Status { get; set; }
    }

    public class GetTimesheetsByCompanyInput : PagedAndSortedResultRequestDto
    {
        public Guid CompanyId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimesheetStatus? Status { get; set; }
    }
}