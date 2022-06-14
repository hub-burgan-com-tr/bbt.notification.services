namespace Notification.Profile.Business
{
    public interface IConsumer
    {
        GetUserConsumersResponse GetUserConsumers(long client,long user,int? source);
    }
}
