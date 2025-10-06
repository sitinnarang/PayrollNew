using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Account.Web.Pages.Account;
using Volo.Abp.Account.Web;
using Volo.Abp.Identity;
using System.Security.Claims;
using System;

namespace PayrollPro.Web.Pages.Account
{
    public class LoginModel : Volo.Abp.Account.Web.Pages.Account.LoginModel
    {
        private readonly UserManager<Volo.Abp.Identity.IdentityUser> _userManager;

        public LoginModel(IAuthenticationSchemeProvider schemeProvider, 
            IOptions<AbpAccountOptions> accountOptions,
            IOptions<IdentityOptions> identityOptions,
            IdentityDynamicClaimsPrincipalContributorCache contributorCache,
            IWebHostEnvironment hostingEnvironment,
            UserManager<Volo.Abp.Identity.IdentityUser> userManager) 
            : base(schemeProvider, accountOptions, identityOptions, contributorCache, hostingEnvironment)
        {
            _userManager = userManager;
        }

        public override async Task<IActionResult> OnGetAsync()
        {
            return await base.OnGetAsync();
        }

        public override async Task<IActionResult> OnPostAsync(string action)
        {
            // Call the base login logic first
            var result = await base.OnPostAsync(action);

            // Check if login was successful (result is a redirect)
            if (result is RedirectResult || result is LocalRedirectResult)
            {
                // Get the current user after successful login
                if (HttpContext.User.Identity?.IsAuthenticated == true)
                {
                    var companyIdClaim = HttpContext.User.FindFirst("CompanyId");
                    var userTypeClaim = HttpContext.User.FindFirst("UserType");

                    // If this is a company user, redirect to their company profile
                    if (companyIdClaim != null && userTypeClaim?.Value == "CompanyUser")
                    {
                        var companyId = companyIdClaim.Value;
                        return Redirect($"/Companies/{companyId}");
                    }
                }
            }

            return result;
        }
    }
}