using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PayrollPro.Timesheets;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace PayrollPro.Web.Pages.Timesheets
{
    public class IndexModel : AbpPageModel
    {
        private readonly ITimesheetAppService _timesheetAppService;

        [BindProperty(SupportsGet = true)]
        public Guid CompanyId { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? EmployeeId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime WeekStarting { get; set; }

        public TimesheetDto CurrentTimesheet { get; set; }

        public IndexModel(ITimesheetAppService timesheetAppService)
        {
            _timesheetAppService = timesheetAppService;
        }

        public async Task OnGetAsync()
        {
            // Default to current week if not specified
            if (WeekStarting == default)
            {
                WeekStarting = GetStartOfWeek(DateTime.Today);
            }

            // If employee is specified, load their timesheet
            if (EmployeeId.HasValue)
            {
                try
                {
                    CurrentTimesheet = await _timesheetAppService.GetWeeklyTimesheetAsync(EmployeeId.Value, WeekStarting);
                }
                catch
                {
                    // Create empty timesheet if none exists
                    CurrentTimesheet = new TimesheetDto
                    {
                        EmployeeId = EmployeeId.Value,
                        WeekStarting = WeekStarting,
                        WeekEnding = WeekStarting.AddDays(6),
                        Status = TimesheetStatus.Draft
                    };
                }
            }
        }

        public async Task<IActionResult> OnPostSubmitTimesheetAsync()
        {
            if (CurrentTimesheet?.Id != null && CurrentTimesheet.Id != Guid.Empty)
            {
                await _timesheetAppService.SubmitAsync(CurrentTimesheet.Id);
            }
            
            return RedirectToPage(new { CompanyId, EmployeeId, WeekStarting });
        }

        private DateTime GetStartOfWeek(DateTime date)
        {
            var dayOfWeek = (int)date.DayOfWeek;
            return date.AddDays(-dayOfWeek);
        }
    }
}