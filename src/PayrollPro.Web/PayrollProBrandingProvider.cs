using Microsoft.Extensions.Localization;
using PayrollPro.Localization;
using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace PayrollPro.Web;

[Dependency(ReplaceServices = true)]
public class PayrollProBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<PayrollProResource> _localizer;

    public PayrollProBrandingProvider(IStringLocalizer<PayrollProResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
