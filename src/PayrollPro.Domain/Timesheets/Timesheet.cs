using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace PayrollPro.Timesheets
{
    public class Timesheet : FullAuditedAggregateRoot<Guid>
    {
        [Required]
        public Guid EmployeeId { get; set; }

        [Required]
        public DateTime WorkDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Range(0, 24)]
        public decimal HoursWorked { get; set; }

        [Range(0, 24)]
        public decimal OvertimeHours { get; set; }

        [Range(0, 24)]
        public decimal BreakHours { get; set; }

        [Required]
        public TimesheetStatus Status { get; set; } = TimesheetStatus.Draft;

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(100)]
        public string? ProjectCode { get; set; }

        protected Timesheet()
        {
        }

        public Timesheet(
            Guid id,
            Guid employeeId,
            DateTime workDate,
            TimeSpan startTime,
            TimeSpan endTime,
            string? projectCode = null,
            string? notes = null) : base(id)
        {
            EmployeeId = employeeId;
            WorkDate = workDate;
            StartTime = startTime;
            EndTime = endTime;
            ProjectCode = projectCode;
            Notes = notes;
            Status = TimesheetStatus.Draft;

            CalculateHours();
        }

        public void CalculateHours()
        {
            var totalTime = EndTime - StartTime;
            var totalHours = (decimal)totalTime.TotalHours - BreakHours;
            
            if (totalHours > 8)
            {
                HoursWorked = 8;
                OvertimeHours = totalHours - 8;
            }
            else
            {
                HoursWorked = totalHours;
                OvertimeHours = 0;
            }
        }

        public void UpdateTimes(TimeSpan startTime, TimeSpan endTime, decimal breakHours)
        {
            StartTime = startTime;
            EndTime = endTime;
            BreakHours = breakHours;
            CalculateHours();
        }

        public void UpdateStatus(TimesheetStatus status)
        {
            Status = status;
        }

        public void Approve()
        {
            Status = TimesheetStatus.Approved;
        }

        public void Reject(string? notes = null)
        {
            Status = TimesheetStatus.Rejected;
            if (!string.IsNullOrEmpty(notes))
            {
                Notes = notes;
            }
        }
    }
}