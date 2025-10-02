using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace PayrollPro.Companies
{
    public interface ICompanyAppService : ICrudAppService<
        CompanyDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateCompanyDto>
    {
        Task<PayrollSettingsDto> GetPayrollSettingsAsync(Guid id);
        Task<PayrollSettingsDto> UpdatePayrollSettingsAsync(Guid id, PayrollSettingsDto input);
    }
}