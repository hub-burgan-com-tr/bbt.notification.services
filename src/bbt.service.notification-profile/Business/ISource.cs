namespace Notification.Profile.Business
{
    public interface ISource
    {
        GetSourcesResponse GetSources();
        GetSourceTopicByIdResponse GetSourceById(int id);

        void Post(PostSourceRequest data);

        SourceResponseModel Patch(int id, PatchSourceRequest data);

        SourceResponseModel Delete(int id);

        GetSourceConsumersResponse GetSourceConsumers(GetSourceConsumersRequestBody requestModel);

    }
}
