using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayrollPro.Employees;
using PayrollPro.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace PayrollPro.Timesheets
{
    public class TimesheetAppService : CrudAppService<
        Timesheet,
        TimesheetDto,
        Guid,
        GetTimesheetsInput,
        CreateUpdateTimesheetDto>, ITimesheetAppService
    {
        private readonly IRepository<Employee, Guid> _employeeRepository;

        public TimesheetAppService(
            IRepository<Timesheet, Guid> repository,
            IRepository<Employee, Guid> employeeRepository)
            : base(repository)
        {
            _employeeRepository = employeeRepository;
            GetPolicyName = PayrollProPermissions.Timesheets.Default;
            GetListPolicyName = PayrollProPermissions.Timesheets.Default;
            CreatePolicyName = PayrollProPermissions.Timesheets.Create;
            UpdatePolicyName = PayrollProPermissions.Timesheets.Edit;
            DeletePolicyName = PayrollProPermissions.Timesheets.Delete;
        }

        public async Task<TimesheetDto> GetWeeklyTimesheetAsync(Guid employeeId, DateTime weekStarting)
        {
            var weekEnding = weekStarting.AddDays(6);
            
            // Get all timesheets for the week
            var timesheets = await Repository.GetListAsync(t => 
                t.EmployeeId == employeeId && 
                t.WorkDate >= weekStarting && 
                t.WorkDate <= weekEnding);

            var employee = await _employeeRepository.GetAsync(employeeId);

            // Create weekly timesheet DTO
            var weeklyTimesheet = new TimesheetDto
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                WeekStarting = weekStarting,
                WeekEnding = weekEnding,
                Status = timesheets.Any() ? timesheets.First().Status : TimesheetStatus.Draft
            };

            // Map daily timesheets to weekly format
            foreach (var timesheet in timesheets)
            {
                var dayOfWeek = timesheet.WorkDate.DayOfWeek;
                switch (dayOfWeek)
                {
                    case DayOfWeek.Monday:
                        weeklyTimesheet.MondayStart = timesheet.StartTime;
                        weeklyTimesheet.MondayEnd = timesheet.EndTime;
                        break;
                    case DayOfWeek.Tuesday:
                        weeklyTimesheet.TuesdayStart = timesheet.StartTime;
                        weeklyTimesheet.TuesdayEnd = timesheet.EndTime;
                        break;
                    case DayOfWeek.Wednesday:
                        weeklyTimesheet.WednesdayStart = timesheet.StartTime;
                        weeklyTimesheet.WednesdayEnd = timesheet.EndTime;
                        break;
                    case DayOfWeek.Thursday:
                        weeklyTimesheet.ThursdayStart = timesheet.StartTime;
                        weeklyTimesheet.ThursdayEnd = timesheet.EndTime;
                        break;
                    case DayOfWeek.Friday:
                        weeklyTimesheet.FridayStart = timesheet.StartTime;
                        weeklyTimesheet.FridayEnd = timesheet.EndTime;
                        break;
                    case DayOfWeek.Saturday:
                        weeklyTimesheet.SaturdayStart = timesheet.StartTime;
                        weeklyTimesheet.SaturdayEnd = timesheet.EndTime;
                        break;
                    case DayOfWeek.Sunday:
                        weeklyTimesheet.SundayStart = timesheet.StartTime;
                        weeklyTimesheet.SundayEnd = timesheet.EndTime;
                        break;
                }
            }

            // Calculate totals
            weeklyTimesheet.TotalRegularHours = timesheets.Sum(t => t.HoursWorked);
            weeklyTimesheet.TotalOvertimeHours = timesheets.Sum(t => t.OvertimeHours);

            return weeklyTimesheet;
        }

        public async Task<PagedResultDto<TimesheetDto>> GetTimesheetsByCompanyAsync(GetTimesheetsByCompanyInput input)
        {
            var employees = await _employeeRepository.GetListAsync(e => e.CompanyId == input.CompanyId);
            var employeeIds = employees.Select(e => e.Id).ToList();

            var query = await Repository.GetQueryableAsync();
            
            query = query.Where(t => employeeIds.Contains(t.EmployeeId));

            if (input.StartDate.HasValue)
                query = query.Where(t => t.WorkDate >= input.StartDate.Value);

            if (input.EndDate.HasValue)
                query = query.Where(t => t.WorkDate <= input.EndDate.Value);

            if (input.Status.HasValue)
                query = query.Where(t => t.Status == input.Status.Value);

            var totalCount = query.Count();
            var timesheets = query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToList();

            var timesheetDtos = new List<TimesheetDto>();

            foreach (var timesheet in timesheets)
            {
                var employee = employees.First(e => e.Id == timesheet.EmployeeId);
                var dto = ObjectMapper.Map<Timesheet, TimesheetDto>(timesheet);
                dto.EmployeeName = $"{employee.FirstName} {employee.LastName}";
                timesheetDtos.Add(dto);
            }

            return new PagedResultDto<TimesheetDto>(totalCount, timesheetDtos);
        }

        public async Task<TimesheetDto> SubmitAsync(Guid id)
        {
            await CheckUpdatePolicyAsync();
            
            var timesheet = await Repository.GetAsync(id);
            timesheet.UpdateStatus(TimesheetStatus.Submitted);
            
            await Repository.UpdateAsync(timesheet);
            return ObjectMapper.Map<Timesheet, TimesheetDto>(timesheet);
        }

        public async Task<TimesheetDto> ApproveAsync(Guid id)
        {
            await CheckPolicyAsync(PayrollProPermissions.Timesheets.Approve);
            
            var timesheet = await Repository.GetAsync(id);
            timesheet.Approve();
            
            await Repository.UpdateAsync(timesheet);
            return ObjectMapper.Map<Timesheet, TimesheetDto>(timesheet);
        }

        public async Task<TimesheetDto> RejectAsync(Guid id, string notes)
        {
            await CheckPolicyAsync(PayrollProPermissions.Timesheets.Approve);
            
            var timesheet = await Repository.GetAsync(id);
            timesheet.Reject(notes);
            
            await Repository.UpdateAsync(timesheet);
            return ObjectMapper.Map<Timesheet, TimesheetDto>(timesheet);
        }

        protected override async Task<IQueryable<Timesheet>> CreateFilteredQueryAsync(GetTimesheetsInput input)
        {
            var query = await base.CreateFilteredQueryAsync(input);

            if (input.EmployeeId.HasValue)
                query = query.Where(t => t.EmployeeId == input.EmployeeId.Value);

            if (input.StartDate.HasValue)
                query = query.Where(t => t.WorkDate >= input.StartDate.Value);

            if (input.EndDate.HasValue)
                query = query.Where(t => t.WorkDate <= input.EndDate.Value);

            if (input.Status.HasValue)
                query = query.Where(t => t.Status == input.Status.Value);

            return query;
        }

        public override async Task<TimesheetDto> GetAsync(Guid id)
        {
            var timesheet = await Repository.GetAsync(id);
            var employee = await _employeeRepository.GetAsync(timesheet.EmployeeId);
            
            var dto = ObjectMapper.Map<Timesheet, TimesheetDto>(timesheet);
            dto.EmployeeName = $"{employee.FirstName} {employee.LastName}";
            
            return dto;
        }
    }
}