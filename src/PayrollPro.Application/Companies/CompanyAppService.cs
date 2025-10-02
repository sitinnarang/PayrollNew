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