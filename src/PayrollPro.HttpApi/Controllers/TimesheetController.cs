using Microsoft.AspNetCore.Mvc;
using PayrollPro.Timesheets;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace PayrollPro.Controllers
{
    [RemoteService(Name = "Default")]
    [Area("app")]
    [Route("api/app/timesheet")]
    public class TimesheetController : AbpControllerBase, ITimesheetAppService
    {
        private readonly ITimesheetAppService _timesheetAppService;

        public TimesheetController(ITimesheetAppService timesheetAppService)
        {
            _timesheetAppService = timesheetAppService;
        }

        [HttpGet("{id}")]
        public virtual Task<TimesheetDto> GetAsync(Guid id)
        {
            return _timesheetAppService.GetAsync(id);
        }

        [HttpGet]
        public virtual Task<PagedResultDto<TimesheetDto>> GetListAsync(GetTimesheetsInput input)
        {
            return _timesheetAppService.GetListAsync(input);
        }

        [HttpPost]
        public virtual Task<TimesheetDto> CreateAsync(CreateUpdateTimesheetDto input)
        {
            return _timesheetAppService.CreateAsync(input);
        }

        [HttpPut("{id}")]
        public virtual Task<TimesheetDto> UpdateAsync(Guid id, CreateUpdateTimesheetDto input)
        {
            return _timesheetAppService.UpdateAsync(id, input);
        }

        [HttpDelete("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _timesheetAppService.DeleteAsync(id);
        }

        [HttpPost("{id}/submit")]
        public virtual Task<TimesheetDto> SubmitAsync(Guid id)
        {
            return _timesheetAppService.SubmitAsync(id);
        }

        [HttpPost("{id}/approve")]
        public virtual Task<TimesheetDto> ApproveAsync(Guid id)
        {
            return _timesheetAppService.ApproveAsync(id);
        }

        [HttpPost("{id}/reject")]
        public virtual Task<TimesheetDto> RejectAsync(Guid id, string notes)
        {
            return _timesheetAppService.RejectAsync(id, notes);
        }

        [HttpGet("weekly")]
        public virtual Task<TimesheetDto> GetWeeklyTimesheetAsync(Guid employeeId, DateTime weekStarting)
        {
            return _timesheetAppService.GetWeeklyTimesheetAsync(employeeId, weekStarting);
        }

        [HttpGet("company")]
        public virtual Task<PagedResultDto<TimesheetDto>> GetTimesheetsByCompanyAsync(GetTimesheetsByCompanyInput input)
        {
            return _timesheetAppService.GetTimesheetsByCompanyAsync(input);
        }
    }
}