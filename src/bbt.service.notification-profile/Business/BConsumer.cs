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
                        Email = c.Email,
                        DefinitionCode=c.DefinitionCode
                        
                    }

                ).ToList();
            }
            return returnValue;
        }
        public PostConsumerResponse PostConsumers(long client,long sourceId,PostConsumerRequest consumer)
        {
            PostConsumerResponse returnValue = new PostConsumerResponse();
            using (var db = new DatabaseContext())
            {
              var consumers=  db.Consumers.FirstOrDefault(x => x.Client == client && x.SourceId == sourceId);
                if (consumers != null)
                {
                   
                    var  result = db.Database.ExecuteSqlInterpolated($@"UPDATE [Consumers] 
                        SET DefinitionCode = {consumer.DefinitionCode},  
                            IsPushEnabled = {consumer.IsPushEnabled} ,
                            IsSmsEnabled = {consumer.IsSmsEnabled} ,
                            IsEmailEnabled={consumer.IsEmailEnabled}
                        WHERE SourceId = {sourceId} AND
                              Client = {client}  
                             ");
                }
                else
                {
                    db.Add(new Consumer
                    {
                        DefinitionCode = consumer.DefinitionCode,
                        Client = client,
                        User = consumer.User,
                        Email = consumer.Email,
                        DeviceKey = consumer.DeviceKey,
                        IsEmailEnabled = consumer.IsEmailEnabled,
                        IsPushEnabled = consumer.IsPushEnabled,
                        IsSmsEnabled = consumer.IsSmsEnabled,
                        Phone = consumer.Phone,
                        SourceId = (int)sourceId,
                        Filter = consumer.Filter,

                    });
                }

                db.SaveChanges();

                db.NotificationLogs.Add(new NotificationLog
                {
                    IsEmailEnabled = consumer.IsEmailEnabled,
                    Client = client,
                    IsPushEnabled = consumer.IsPushEnabled,
                    IsSmsEnabled = consumer.IsSmsEnabled,
                    SourceId = (int)sourceId,
                    Filter = consumer.Filter,
                    User = consumer.User
                }) ;
                db.SaveChanges();
            }
            returnValue.Result = Enum.ResultEnum.Success;
            return returnValue;
        }
    }
}
