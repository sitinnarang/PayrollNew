using System;
using System.Collections.Generic;
using System.Text;
using PayrollPro.Localization;
using Volo.Abp.Application.Services;

namespace PayrollPro;

/* Inherit your application services from this class.
 */
public abstract class PayrollProAppService : ApplicationService
{
    protected PayrollProAppService()
    {
        LocalizationResource = typeof(PayrollProResource);
    }
}
