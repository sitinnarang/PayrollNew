using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.PermissionManagement;

namespace PayrollPro;

public class PayrollProIdentityDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly IIdentityRoleRepository _roleRepository;
    private readonly IIdentityUserRepository _userRepository;
    private readonly ILookupNormalizer _lookupNormalizer;
    private readonly IdentityUserManager _userManager;
    private readonly IdentityRoleManager _roleManager;
    private readonly ICurrentTenant _currentTenant;
    private readonly IOptions<IdentityOptions> _identityOptions;
    private readonly IPermissionManager _permissionManager;

    public PayrollProIdentityDataSeedContributor(
        IGuidGenerator guidGenerator,
        IIdentityRoleRepository roleRepository,
        IIdentityUserRepository userRepository,
        ILookupNormalizer lookupNormalizer,
        IdentityUserManager userManager,
        IdentityRoleManager roleManager,
        ICurrentTenant currentTenant,
        IOptions<IdentityOptions> identityOptions,
        IPermissionManager permissionManager)
    {
        _guidGenerator = guidGenerator;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _lookupNormalizer = lookupNormalizer;
        _userManager = userManager;
        _roleManager = roleManager;
        _currentTenant = currentTenant;
        _identityOptions = identityOptions;
        _permissionManager = permissionManager;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        using (_currentTenant.Change(context?.TenantId))
        {
            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedPermissionsAsync();
        }
    }

    private async Task SeedRolesAsync()
    {
        var adminRoleName = "admin";
        
        if (await _roleRepository.FindByNormalizedNameAsync(_lookupNormalizer.NormalizeName(adminRoleName)) == null)
        {
            await _roleRepository.InsertAsync(
                new IdentityRole(
                    _guidGenerator.Create(),
                    adminRoleName,
                    _currentTenant.Id
                )
                {
                    IsStatic = true,
                    IsPublic = true
                }, 
                autoSave: true
            );
        }
    }

    private async Task SeedUsersAsync()
    {
        var adminUserName = "admin";
        var adminEmail = "admin@abp.io";
        var adminPassword = "1q2w3E*";

        if (await _userRepository.FindByNormalizedUserNameAsync(_lookupNormalizer.NormalizeName(adminUserName)) == null)
        {
            var adminUser = new IdentityUser(
                _guidGenerator.Create(),
                adminUserName,
                adminEmail,
                _currentTenant.Id
            );

            adminUser.Name = "Admin";
            adminUser.Surname = "User";
            adminUser.SetEmailConfirmed(true);

            var result = await _userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "admin");
            }
        }
    }

    private async Task SeedPermissionsAsync()
    {
        // Grant admin permissions to admin role
        var adminRole = await _roleRepository.FindByNormalizedNameAsync(_lookupNormalizer.NormalizeName("admin"));
        if (adminRole != null)
        {
            // Grant all admin permissions
            await _permissionManager.SetForRoleAsync("admin", "PayrollPro.Admin", true);
            await _permissionManager.SetForRoleAsync("admin", "PayrollPro.Admin.ManageAllCompanies", true);
            await _permissionManager.SetForRoleAsync("admin", "PayrollPro.Admin.ViewAllCompanies", true);
            
            // Grant all other permissions for a super admin
            await _permissionManager.SetForRoleAsync("admin", "PayrollPro.Companies", true);
            await _permissionManager.SetForRoleAsync("admin", "PayrollPro.Companies.Create", true);
            await _permissionManager.SetForRoleAsync("admin", "PayrollPro.Companies.Edit", true);
            await _permissionManager.SetForRoleAsync("admin", "PayrollPro.Companies.Delete", true);
            await _permissionManager.SetForRoleAsync("admin", "PayrollPro.Companies.ViewAll", true);
        }
    }
}