using Notification.Profile.Business;
using bbt.service.notification_profile.Helper;

namespace bbt.framework.dengage.Business
{
    public class BGetCustomerInfo : BaseRefit<IGetCustomerInfo>
    {
         static ConfigurationHelper configurationHelper=new ConfigurationHelper();
        public BGetCustomerInfo(ILogger logger):base(configurationHelper.GetCustomerProfileEndpoint(), string.Empty, logger)
        {

        }
        public override string controllerName => "CustomerSearch";

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
