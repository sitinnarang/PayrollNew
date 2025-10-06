using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace PayrollPro.Web.Middleware
{
    public class CompanyRedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public CompanyRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<Volo.Abp.Identity.IdentityUser> userManager)
        {
            // Check if the user just logged in and is being redirected to home page
            if (context.Request.Path == "/" && 
                context.User.Identity?.IsAuthenticated == true && 
                context.User.Identity.Name != null)
            {
                // Check if user has a company claim
                var companyIdClaim = context.User.FindFirst("CompanyId");
                
                if (companyIdClaim != null)
                {
                    // Redirect to their company page instead of home
                    context.Response.Redirect($"/Companies/{companyIdClaim.Value}");
                    return;
                }
            }

            await _next(context);
        }
    }
}