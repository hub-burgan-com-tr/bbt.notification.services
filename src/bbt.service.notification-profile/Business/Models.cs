namespace Notification.Profile.Business
{
    public class CustomerInformationModel

    {

        public int recordCount { get; set; }
        public int returnCode { get; set; }
        public string returnDescription { get; set; }
        public List<CustomerList> customerList { get; set; }
    }

    public class GetTelephoneNumberRequestModel

    {
        public int page { get; set; }=1;
        public int size { get; set; }=10;
        public string name { get; set; }
    }
        public class CustomerList
    {
        public int customerNumber { get; set; }
        public Name name { get; set; }
        public string citizenshipNumber { get; set; }
        public string taxNo { get; set; }
        public bool isStaff { get; set; }
        public GsmPhone gsmPhone { get; set; }
        public string email { get; set; }
        public string businessLine { get; set; }
        public object device { get; set; }
        public string identityNumber { get; set; }
    }

    public class GsmPhone
    {
        public string country { get; set; }
        public string prefix { get; set; }
        public string number { get; set; }
    }

    public class Name
    {
        public string first { get; set; }
        public string last { get; set; }
    }
}
