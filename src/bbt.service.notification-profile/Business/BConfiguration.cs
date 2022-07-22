using Microsoft.EntityFrameworkCore;

namespace Notification.Profile.Business
{
    public class BConfiguration : IConfigurations
    {
        private readonly IConfiguration _configuration;

        public BConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public GetConsumerTreeResponse GetUserConsumers(long client, long user)
        {
            GetConsumerTreeResponse returnValue = new GetConsumerTreeResponse();
            using (var db = new DatabaseContext())
            {
                var consumers = db.Consumers.Where(s =>
                    s.Client == client &&
                    s.User == user
                ).ToList();

                var sources = db.Sources.Select(x => x.Id);
                List<GetConsumerTreeResponse.ConfUser> confUsers = new List<GetConsumerTreeResponse.ConfUser>();


                foreach (var sourceId in sources)
                {
                    consumers = null;
                    var consumer = consumers.FirstOrDefault(x => sourceId == x.SourceId);

                    if (consumer != null)
                    {
                        confUsers.Add(new GetConsumerTreeResponse.ConfUser
                        {
                            Source = consumer.SourceId,
                            Filter = consumer.Filter,
                            IsPushEnabled = consumer.IsPushEnabled,
                            DeviceKey = consumer.DeviceKey,
                            IsSmsEnabled = consumer.IsSmsEnabled,
                            Phone = consumer.Phone,
                            IsEmailEnabled = consumer.IsEmailEnabled,
                            Email = consumer.Email
                        });


                    }

                    else
                    {
                        confUsers.Add(new GetConsumerTreeResponse.ConfUser
                        {
                            Source = sourceId,
                            Filter = null,
                            IsPushEnabled = false,
                            DeviceKey = null,
                            IsSmsEnabled = false,
                            Phone = null,
                            IsEmailEnabled = false,
                            Email = null
                        });

                    }
                }

                returnValue.Consumers = confUsers;
            }
            return returnValue;
        }

        public GetClientUsersResponse GetUsers(long client)
        {

            GetClientUsersResponse returnValue = new GetClientUsersResponse();
            using (var db = new DatabaseContext())
            {
                var users = db.Consumers.Where(s => s.Client == client)
                    .Select(m => m.User)
                    .Distinct()
                    .ToList();

                returnValue.Users = users;

            }
            return returnValue;

        }

        public GetUserConsumersResponse PostUserConsumers(long client, long user)
        {
            throw new NotImplementedException();
        }

       

        public PostUpdateResponse UpdateDevice(long user, PostUpdateDeviceRequest data)
        {
            int result = 0;
            using (var db = new DatabaseContext())
            {

                result = db.Database.ExecuteSqlInterpolated($"UPDATE [Consumers] SET DeviceKey = '{data.NewDeviceKey}' WHERE DeviceKey = '{data.OldDeviceKey}' AND [User] = {user}");
                return new PostUpdateResponse { UpdatedRecordCount = result };
            }

        }

        public PostUpdateResponse UpdateEmail(long user, PostUpdateEmailRequest data)
        {
            int result = 0;
            using (var db = new DatabaseContext())
            {
                result = db.Database.ExecuteSqlInterpolated($"UPDATE [Consumers] SET Email = '{data.NewEmail}' WHERE Email = '{data.OldEmail}' AND [User] = {user}");

                return new PostUpdateResponse { UpdatedRecordCount = result };
            }

        }

        public PostUpdateResponse UpdatePhone(long user, PostUpdatePhoneRequest data)
        {
            int result = 0;
            using (var db = new DatabaseContext())
            {
                result = db.Database.ExecuteSqlInterpolated($@"UPDATE [Consumers] 
                        SET Phone_CountryCode = {data.NewPhone.CountryCode},  
                            Phone_Prefix = {data.NewPhone.Prefix},
                            Phone_Number = {data.NewPhone.Number} 
                        WHERE Phone_CountryCode = {data.OldPhone.CountryCode} AND
                              Phone_Prefix = {data.OldPhone.Prefix} AND 
                              Phone_Number = {data.OldPhone.Number} AND 
                              [User] = {user}");

                return new PostUpdateResponse { UpdatedRecordCount = result };
            }
        }
    }
}
