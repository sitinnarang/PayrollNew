using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace PayrollPro.Web.Controllers
{
    [Route("api/test")]
    public class TestController : Controller
    {
        private readonly IIdentityUserAppService _identityUserAppService;

        public TestController(IIdentityUserAppService identityUserAppService)
        {
            _identityUserAppService = identityUserAppService;
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateTestUser()
        {
            try
            {
                var userCreateDto = new IdentityUserCreateDto
                {
                    UserName = "testuser" + System.DateTime.Now.Ticks,
                    Email = $"testuser{System.DateTime.Now.Ticks}@example.com",
                    Password = "1q2w3E*", // Same format as admin password
                    IsActive = true,
                    LockoutEnabled = false,
                    Name = "Test",
                    Surname = "User"
                };

                var createdUser = await _identityUserAppService.CreateAsync(userCreateDto);

                return Ok(new { 
                    success = true, 
                    userId = createdUser.Id,
                    username = createdUser.UserName,
                    email = createdUser.Email
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    error = ex.Message,
                    fullError = ex.ToString()
                });
            }
        }
    }
}