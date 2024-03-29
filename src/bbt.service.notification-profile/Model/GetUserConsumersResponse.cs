
using Notification.Profile.Model.BaseResponse;
public class GetUserConsumersResponse: BaseResponseModel
{
    public List<Consumer> Consumers { get; set; }

    public class Consumer
    {
        public int Source { get; set; }
        public string Filter { get; set; }
        public bool IsPushEnabled { get; set; }
        public string DeviceKey { get; set; }
        public bool IsSmsEnabled { get; set; }
        public Phone Phone { get; set; }
        public bool IsEmailEnabled { get; set; }
        public string Email { get; set; }

        public string DefinitionCode { get; set; }
        public bool IsStaff { get; set; }

    }


}
