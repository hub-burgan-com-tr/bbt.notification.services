using Microsoft.EntityFrameworkCore;

namespace Notification.Profile.Business
{
    public class BConsumer:IConsumer
    {
        private readonly IConfiguration _configuration;

        public BConsumer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public GetUserConsumersResponse GetUserConsumers(long client, long user, int? source)
        {
            GetUserConsumersResponse returnValue = new GetUserConsumersResponse();
            using (var db = new DatabaseContext())
            {
                var consumers = db.Consumers.Where(s =>
                    s.Client == client &&
                    s.User == user &&
                    (source == null || source.HasValue || s.SourceId == source.Value))
                .AsNoTracking();
                returnValue.Consumers = consumers.Select(c =>
                    new GetUserConsumersResponse.Consumer
                    {
                        Source = c.SourceId,
                        Filter = c.Filter,
                        IsPushEnabled = c.IsPushEnabled,
                        DeviceKey = c.DeviceKey,
                        IsSmsEnabled = c.IsSmsEnabled,
                        Phone = c.Phone,
                        IsEmailEnabled = c.IsEmailEnabled,
                        Email = c.Email
                    }

                ).ToList();
            }
            return returnValue;
        }
    }
}
