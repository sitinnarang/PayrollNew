using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace PayrollPro.Employees
{
    public interface IEmployeeAppService : ICrudAppService<
        EmployeeDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateEmployeeDto>
    {
        Task<EmployeeDto> GetByEmployeeIdAsync(string employeeId);
        Task<ListResultDto<EmployeeDto>> GetEmployeesByDepartmentAsync(string department);
        Task<ListResultDto<EmployeeDto>> GetEmployeesByStatusAsync(EmployeeStatus status);
    }
}