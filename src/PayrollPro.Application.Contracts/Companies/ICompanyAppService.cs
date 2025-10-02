using System;
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
    }
}