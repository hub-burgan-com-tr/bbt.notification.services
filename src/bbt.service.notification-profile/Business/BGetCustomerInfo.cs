using Notification.Profile.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.framework.dengage.Business
{






    public class BGetCustomerInfo : BaseRefit<IGetCustomerInfo>
    {
        public BGetCustomerInfo(ILogger logger):base("https://test-notification-enrichment.burgan.com.tr", string.Empty, logger)
        {

        }
        public override string controllerName => "login";

        public async Task<CustomerInformationModel> GetTelephoneNumber(GetTelephoneNumberRequestModel request)
        {
            return await ExecutePolly(() =>
            {
                return api.GetTelephoneNumber(request).Result;
            }
            );
        }


    }
}
