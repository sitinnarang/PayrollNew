using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayrollPro.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace PayrollPro.Employees
{
    public class EmployeeAppService : CrudAppService<
        Employee,
        EmployeeDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateEmployeeDto>, IEmployeeAppService
    {
        public EmployeeAppService(IRepository<Employee, Guid> repository)
            : base(repository)
        {
            GetPolicyName = PayrollProPermissions.Employees.Default;
            GetListPolicyName = PayrollProPermissions.Employees.Default;
            CreatePolicyName = PayrollProPermissions.Employees.Create;
            UpdatePolicyName = PayrollProPermissions.Employees.Edit;
            DeletePolicyName = PayrollProPermissions.Employees.Delete;
        }

        public async Task<EmployeeDto> GetByEmployeeIdAsync(string employeeId)
        {
            var employee = await Repository.GetAsync(x => x.EmployeeId == employeeId);
            return await MapToGetOutputDtoAsync(employee);
        }

        public async Task<ListResultDto<EmployeeDto>> GetEmployeesByDepartmentAsync(string department)
        {
            var employees = await Repository.GetListAsync(x => x.Department == department);
            return new ListResultDto<EmployeeDto>(
                ObjectMapper.Map<List<Employee>, List<EmployeeDto>>(employees)
            );
        }

        public async Task<ListResultDto<EmployeeDto>> GetEmployeesByStatusAsync(EmployeeStatus status)
        {
            var employees = await Repository.GetListAsync(x => x.Status == status);
            return new ListResultDto<EmployeeDto>(
                ObjectMapper.Map<List<Employee>, List<EmployeeDto>>(employees)
            );
        }

        public override async Task<EmployeeDto> CreateAsync(CreateUpdateEmployeeDto input)
        {
            // Check if employee ID already exists
            if (await Repository.AnyAsync(x => x.EmployeeId == input.EmployeeId))
            {
                throw new Volo.Abp.BusinessException(PayrollProDomainErrorCodes.EmployeeIdAlreadyExists)
                    .WithData("EmployeeId", input.EmployeeId);
            }

            // Check if email already exists
            if (await Repository.AnyAsync(x => x.Email == input.Email))
            {
                throw new Volo.Abp.BusinessException(PayrollProDomainErrorCodes.EmployeeEmailAlreadyExists)
                    .WithData("Email", input.Email);
            }

            return await base.CreateAsync(input);
        }

        public override async Task<EmployeeDto> UpdateAsync(Guid id, CreateUpdateEmployeeDto input)
        {
            // Check if employee ID already exists (excluding current employee)
            if (await Repository.AnyAsync(x => x.EmployeeId == input.EmployeeId && x.Id != id))
            {
                throw new Volo.Abp.BusinessException(PayrollProDomainErrorCodes.EmployeeIdAlreadyExists)
                    .WithData("EmployeeId", input.EmployeeId);
            }

            // Check if email already exists (excluding current employee)
            if (await Repository.AnyAsync(x => x.Email == input.Email && x.Id != id))
            {
                throw new Volo.Abp.BusinessException(PayrollProDomainErrorCodes.EmployeeEmailAlreadyExists)
                    .WithData("Email", input.Email);
            }

            return await base.UpdateAsync(id, input);
        }
    }
}