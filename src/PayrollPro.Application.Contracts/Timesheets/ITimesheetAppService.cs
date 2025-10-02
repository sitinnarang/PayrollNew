using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace PayrollPro.Timesheets
{
    public interface ITimesheetAppService : IApplicationService
    {
        Task<TimesheetDto> GetAsync(Guid id);
        Task<PagedResultDto<TimesheetDto>> GetListAsync(GetTimesheetsInput input);
        Task<TimesheetDto> CreateAsync(CreateUpdateTimesheetDto input);
        Task<TimesheetDto> UpdateAsync(Guid id, CreateUpdateTimesheetDto input);
        Task DeleteAsync(Guid id);
        Task<TimesheetDto> SubmitAsync(Guid id);
        Task<TimesheetDto> ApproveAsync(Guid id);
        Task<TimesheetDto> RejectAsync(Guid id, string notes);
        Task<TimesheetDto> GetWeeklyTimesheetAsync(Guid employeeId, DateTime weekStarting);
        Task<PagedResultDto<TimesheetDto>> GetTimesheetsByCompanyAsync(GetTimesheetsByCompanyInput input);
    }
}