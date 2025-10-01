using PayrollPro.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace PayrollPro.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class PayrollProPageModel : AbpPageModel
{
    protected PayrollProPageModel()
    {
        LocalizationResourceType = typeof(PayrollProResource);
    }
}
