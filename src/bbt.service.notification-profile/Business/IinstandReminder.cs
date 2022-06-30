namespace Notification.Profile.Business
{
    public interface IinstandReminder
    {
        GetInstantCustomerPermissionResponse GetCustomerPermission(string customerId,string lang);

    }
}