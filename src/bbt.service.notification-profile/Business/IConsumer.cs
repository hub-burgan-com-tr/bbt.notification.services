namespace Notification.Profile.Business
{
    public interface IConsumer
    {
        GetUserConsumersResponse GetUserConsumers(long client,long user,int? source);
        PostConsumerResponse PostConsumers(long client,long sourceId,PostConsumerRequest consumer);
    }
}
