using Notification.Profile.Business;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbt.framework.dengage.Business
{
    public interface IGetCustomerInfo
    {
        [Post("/Customer/GetTelephoneNumber")]        
        Task<CustomerInformationModel> GetTelephoneNumber([Body(BodySerializationMethod.Serialized)] GetTelephoneNumberRequestModel model);
    }
}
