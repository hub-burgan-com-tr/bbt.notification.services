namespace Notification.Profile.Business
{
    public interface ISource
    {
        GetSourcesResponse GetSources();
        GetSourceTopicByIdResponse GetSourceById(int id);

        void Post(PostSourceRequest data);

        void Patch(int id, PatchSourceRequest data);

        void Delete(int id);

        GetSourceConsumersResponse GetSourceConsumers(GetSourceConsumersRequestBody requestModel);

    }
}
