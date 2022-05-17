namespace Notification.Profile.Business
{
    public class CustomerInformationModel

    {

        public int countryCode { get; set; }

        public int cityCode { get; set; }

        public int telephoneNumber { get; set; }

        public long CustomerNo { get; set; }



    }

    public class GetTelephoneNumberRequestModel

    {

        public long customerId { get; set; }

    }
}
