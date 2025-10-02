using Microsoft.AspNetCore.Mvc;
using PayrollPro.Employees;
using PayrollPro.Permissions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization.Permissions;

namespace PayrollPro.Web.Pages;

public class IndexModel : PayrollProPageModel
{
    private readonly IEmployeeAppService _employeeAppService;
    private readonly IPermissionChecker _permissionChecker;

    public IndexModel(IEmployeeAppService employeeAppService, IPermissionChecker permissionChecker)
    {
        _employeeAppService = employeeAppService;
        _permissionChecker = permissionChecker;
    }

    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public decimal MonthlyPayroll { get; set; }
    public int PendingTimesheets { get; set; }
    public bool IsAdmin { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            // Check if user is admin
            IsAdmin = await _permissionChecker.IsGrantedAsync(PayrollProPermissions.Admin.ViewAllCompanies);

            // Get employee statistics
            var allEmployees = await _employeeAppService.GetListAsync(new PagedAndSortedResultRequestDto { MaxResultCount = 1000 });
            TotalEmployees = (int)allEmployees.TotalCount;
            ActiveEmployees = allEmployees.Items.Count(e => e.Status == PayrollPro.Employees.EmployeeStatus.Active);

            // Set sample data for other metrics (will be implemented later)
            MonthlyPayroll = 125400.00m;
            PendingTimesheets = 3;
        }
        catch
        {
            // Set default values if services are not available yet
            TotalEmployees = 245;
            ActiveEmployees = 230;
            MonthlyPayroll = 125400.00m;
            PendingTimesheets = 3;
            IsAdmin = false;
        }
    }
}
