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

namespace PayrollPro.Web.Pages.Account
{
    public class LoginModel : Volo.Abp.Account.Web.Pages.Account.LoginModel
    {
        public LoginModel(IAuthenticationSchemeProvider schemeProvider, 
            IOptions<AbpAccountOptions> accountOptions,
            IOptions<IdentityOptions> identityOptions,
            IdentityDynamicClaimsPrincipalContributorCache contributorCache,
            IWebHostEnvironment hostingEnvironment) 
            : base(schemeProvider, accountOptions, identityOptions, contributorCache, hostingEnvironment)
        {
        }

        public override async Task<IActionResult> OnGetAsync()
        {
            return await base.OnGetAsync();
        }

        public override async Task<IActionResult> OnPostAsync(string action)
        {
            return await base.OnPostAsync(action);
        }
    }
}