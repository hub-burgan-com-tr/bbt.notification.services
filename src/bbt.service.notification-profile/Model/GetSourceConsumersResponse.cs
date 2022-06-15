

using Notification.Profile.Model.BaseResponseModel;

public class GetSourceConsumersResponse:BaseResponseModel
{
    public List<Consumer> Consumers { get; set; }

    public class Consumer
    {
        public Guid Id { get; set; }
        public long Client { get; set; }
        public long User { get; set; }
        public string Filter { get; set; }
        public bool IsPushEnabled { get; set; }
        public string DeviceKey { get; set; }
        public bool IsSmsEnabled { get; set; }
        public Phone Phone { get; set; }
        public bool IsEmailEnabled { get; set; }
        public string Email { get; set; }
    }
}
