using PayrollPro.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace PayrollPro.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class PayrollProController : AbpControllerBase
{
    protected PayrollProController()
    {
        LocalizationResource = typeof(PayrollProResource);
    }
}
