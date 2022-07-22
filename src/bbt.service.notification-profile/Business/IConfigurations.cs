namespace Notification.Profile.Business
{
    public interface IConfigurations
    {
        GetClientUsersResponse GetUsers(long client);
        GetConsumerTreeResponse GetUserConsumers(long client,long user);

        //GetUserConsumersResponse PostUserConsumers(long client, long user, Consumer consumer, string definitionCode);
        GetUserConsumersResponse PostUserConsumers(long client, long user);

    

        PostUpdateResponse UpdateEmail(long user, PostUpdateEmailRequest data);

        PostUpdateResponse UpdatePhone( long user,PostUpdatePhoneRequest data);

        PostUpdateResponse UpdateDevice(long user, PostUpdateDeviceRequest data);
    }
}
