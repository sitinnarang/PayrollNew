using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace PayrollPro.Companies
{
    public class CompanyAppService : CrudAppService<
            Company,
            CompanyDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateCompanyDto>,
        ICompanyAppService
    {
        public CompanyAppService(IRepository<Company, Guid> repository)
            : base(repository)
        {
        }

        public override async Task<CompanyDto> CreateAsync(CreateUpdateCompanyDto input)
        {
            var company = ObjectMapper.Map<CreateUpdateCompanyDto, Company>(input);
            
            // Generate company code if not provided
            if (string.IsNullOrWhiteSpace(company.Code))
            {
                company.Code = await GenerateCompanyCodeAsync(company.Name);
            }

            await Repository.InsertAsync(company);
            return ObjectMapper.Map<Company, CompanyDto>(company);
        }

        public async Task<PayrollSettingsDto> GetPayrollSettingsAsync(Guid id)
        {
            var company = await Repository.GetAsync(id);
            
            return new PayrollSettingsDto
            {
                PayFrequency = company.PayFrequency,
                PayPeriodEnd = company.PayPeriodEnd,
                StandardWorkHours = company.StandardWorkHours,
                OvertimeRate = company.OvertimeRate,
                AutoProcessPayroll = company.AutoProcessPayroll
            };
        }

        public async Task<PayrollSettingsDto> UpdatePayrollSettingsAsync(Guid id, PayrollSettingsDto input)
        {
            var company = await Repository.GetAsync(id);
            
            company.PayFrequency = input.PayFrequency;
            company.PayPeriodEnd = input.PayPeriodEnd;
            company.StandardWorkHours = input.StandardWorkHours;
            company.OvertimeRate = input.OvertimeRate;
            company.AutoProcessPayroll = input.AutoProcessPayroll;
            
            await Repository.UpdateAsync(company);
            
            return ObjectMapper.Map<Company, PayrollSettingsDto>(company);
        }

        private async Task<string> GenerateCompanyCodeAsync(string companyName)
        {
            // Generate code from company name
            var code = new string(companyName.Where(char.IsLetter).Take(3).ToArray()).ToUpper();
            
            // Add number suffix if code already exists
            var counter = 1;
            var baseCode = code;
            
            while (await Repository.AnyAsync(c => c.Code == code))
            {
                code = $"{baseCode}{counter:D2}";
                counter++;
            }
            
            return code;
        }
    }
}