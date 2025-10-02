using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayrollPro.Companies;
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
        private readonly IRepository<Company, Guid> _companyRepository;

        public EmployeeAppService(
            IRepository<Employee, Guid> repository,
            IRepository<Company, Guid> companyRepository)
            : base(repository)
        {
            _companyRepository = companyRepository;
            GetPolicyName = PayrollProPermissions.Employees.Default;
            GetListPolicyName = PayrollProPermissions.Employees.Default;
            CreatePolicyName = PayrollProPermissions.Employees.Create;
            UpdatePolicyName = PayrollProPermissions.Employees.Edit;
            DeletePolicyName = PayrollProPermissions.Employees.Delete;
        }

        public override async Task<PagedResultDto<EmployeeDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var employees = await Repository.GetListAsync();
            var companies = await _companyRepository.GetListAsync();
            
            var employeeDtos = ObjectMapper.Map<List<Employee>, List<EmployeeDto>>(employees);
            
            // Populate company information
            foreach (var dto in employeeDtos)
            {
                var company = companies.FirstOrDefault(c => c.Id == dto.CompanyId);
                if (company != null)
                {
                    dto.CompanyName = company.Name;
                    dto.CompanyCode = company.Code;
                }
            }
            
            var totalCount = await Repository.GetCountAsync();
            
            return new PagedResultDto<EmployeeDto>(
                totalCount,
                employeeDtos.Skip(input.SkipCount).Take(input.MaxResultCount).ToList()
            );
        }

        public override async Task<EmployeeDto> GetAsync(Guid id)
        {
            var employee = await Repository.GetAsync(id);
            var dto = ObjectMapper.Map<Employee, EmployeeDto>(employee);
            
            // Populate company information
            var company = await _companyRepository.FirstOrDefaultAsync(c => c.Id == dto.CompanyId);
            if (company != null)
            {
                dto.CompanyName = company.Name;
                dto.CompanyCode = company.Code;
            }
            
            return dto;
        }

        public async Task<EmployeeDto> GetByEmployeeIdAsync(string employeeId)
        {
            var employee = await Repository.GetAsync(x => x.EmployeeId == employeeId);
            return await GetAsync(employee.Id);
        }

        public async Task<ListResultDto<EmployeeDto>> GetEmployeesByDepartmentAsync(string department)
        {
            var employees = await Repository.GetListAsync(x => x.Department == department);
            var employeeDtos = ObjectMapper.Map<List<Employee>, List<EmployeeDto>>(employees);
            
            // Populate company information
            var companies = await _companyRepository.GetListAsync();
            foreach (var dto in employeeDtos)
            {
                var company = companies.FirstOrDefault(c => c.Id == dto.CompanyId);
                if (company != null)
                {
                    dto.CompanyName = company.Name;
                    dto.CompanyCode = company.Code;
                }
            }
            
            return new ListResultDto<EmployeeDto>(employeeDtos);
        }

        public async Task<ListResultDto<EmployeeDto>> GetEmployeesByStatusAsync(EmployeeStatus status)
        {
            var employees = await Repository.GetListAsync(x => x.Status == status);
            var employeeDtos = ObjectMapper.Map<List<Employee>, List<EmployeeDto>>(employees);
            
            // Populate company information
            var companies = await _companyRepository.GetListAsync();
            foreach (var dto in employeeDtos)
            {
                var company = companies.FirstOrDefault(c => c.Id == dto.CompanyId);
                if (company != null)
                {
                    dto.CompanyName = company.Name;
                    dto.CompanyCode = company.Code;
                }
            }
            
            return new ListResultDto<EmployeeDto>(employeeDtos);
        }

        public async Task<PagedResultDto<EmployeeDto>> GetEmployeesByCompanyAsync(Guid companyId, PagedAndSortedResultRequestDto input)
        {
            Console.WriteLine($"DEBUG: GetEmployeesByCompanyAsync called with companyId: {companyId}");
            
            // First, let's see all employees and their company IDs
            var allEmployees = await Repository.GetListAsync();
            Console.WriteLine($"DEBUG: Total employees in database: {allEmployees.Count}");
            
            foreach (var emp in allEmployees.Take(5)) // Show first 5
            {
                Console.WriteLine($"DEBUG: Employee {emp.FirstName} {emp.LastName} has CompanyId: {emp.CompanyId}");
            }
            
            var employees = await Repository.GetListAsync(x => x.CompanyId == companyId);
            Console.WriteLine($"DEBUG: Found {employees.Count} employees for company {companyId}");
            
            var employeeDtos = ObjectMapper.Map<List<Employee>, List<EmployeeDto>>(employees);
            
            // Populate company information
            var company = await _companyRepository.FirstOrDefaultAsync(c => c.Id == companyId);
            Console.WriteLine($"DEBUG: Company found: {company?.Name ?? "NOT FOUND"}");
            
            if (company != null)
            {
                foreach (var dto in employeeDtos)
                {
                    dto.CompanyName = company.Name;
                    dto.CompanyCode = company.Code;
                }
            }
            
            var totalCount = employees.Count;
            var pagedEmployees = employeeDtos.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();
            
            Console.WriteLine($"DEBUG: Returning {pagedEmployees.Count} of {totalCount} total employees");
            
            return new PagedResultDto<EmployeeDto>(totalCount, pagedEmployees);
        }

        public override async Task<EmployeeDto> CreateAsync(CreateUpdateEmployeeDto input)
        {
            // Validate company exists
            var company = await _companyRepository.GetAsync(input.CompanyId);
            
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

            var result = await base.CreateAsync(input);
            return await GetAsync(result.Id);
        }

        public override async Task<EmployeeDto> UpdateAsync(Guid id, CreateUpdateEmployeeDto input)
        {
            // Validate company exists
            var company = await _companyRepository.GetAsync(input.CompanyId);
            
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

            var result = await base.UpdateAsync(id, input);
            return await GetAsync(result.Id);
        }
    }
}