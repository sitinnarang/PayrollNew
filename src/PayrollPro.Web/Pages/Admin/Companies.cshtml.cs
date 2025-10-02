using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollPro.Companies;
using PayrollPro.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Authorization.Permissions;

namespace PayrollPro.Web.Pages.Admin;

[Authorize(PayrollProPermissions.Admin.ViewAllCompanies)]
public class CompaniesModel : AbpPageModel
{
    private readonly ICompanyAppService _companyAppService;
    private readonly IPermissionChecker _permissionChecker;

    public CompaniesModel(ICompanyAppService companyAppService, IPermissionChecker permissionChecker)
    {
        _companyAppService = companyAppService;
        _permissionChecker = permissionChecker;
    }

    [BindProperty(SupportsGet = true)]
    public string SearchName { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string SearchCity { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string SearchCountry { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public PagedResultDto<CompanyDto> Companies { get; set; } = new();
    public int TotalCompanies { get; set; }
    public int ActiveCompanies { get; set; }
    public int TotalEmployees { get; set; }
    public int NewCompaniesThisMonth { get; set; }
    public bool CanManageAllCompanies { get; set; }

    public async Task OnGetAsync()
    {
        // Check permissions
        CanManageAllCompanies = await _permissionChecker.IsGrantedAsync(PayrollProPermissions.Admin.ManageAllCompanies);

        // Get filtered companies
        var input = new PagedAndSortedResultRequestDto
        {
            MaxResultCount = 25,
            SkipCount = (CurrentPage - 1) * 25,
            Sorting = "CreationTime desc"
        };

        Companies = await _companyAppService.GetListAsync(input);

        // Calculate statistics
        CalculateStatistics();
    }

    private void CalculateStatistics()
    {
        // For now, we'll use the current page data
        // In a real implementation, you'd want separate API calls for these statistics
        TotalCompanies = (int)Companies.TotalCount;
        ActiveCompanies = TotalCompanies; // Assuming all are active for now
        
        // Calculate total employees across all companies
        TotalEmployees = 0;
        foreach (var company in Companies.Items)
        {
            TotalEmployees += company.EmployeeCount;
        }

        // Calculate new companies this month
        NewCompaniesThisMonth = 0;
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;
        
        foreach (var company in Companies.Items)
        {
            if (company.CreationTime.Month == currentMonth && company.CreationTime.Year == currentYear)
            {
                NewCompaniesThisMonth++;
            }
        }
    }
}