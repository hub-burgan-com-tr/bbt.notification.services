namespace Notification.Profile.Business
{
    public interface IinstandReminder
    {
        Task<GetInstantCustomerPermissionResponse> GetCustomerPermission(string customerId,string lang);

    }
}