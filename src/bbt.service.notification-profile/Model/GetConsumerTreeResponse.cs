using Notification.Profile.Model.BaseResponse;

public class GetConsumerTreeResponse :BaseResponseModel
{
    public List<ConfUser> Consumers { get; set; }

    public class ConfUser
    {
        public int Source { get; set; }
        public string Filter { get; set; }
        public bool IsPushEnabled { get; set; }
        public string DeviceKey { get; set; }
        public bool IsSmsEnabled { get; set; }
        public Phone Phone { get; set; }
        public bool IsEmailEnabled { get; set; }
        public string Email { get; set; }

        public bool IsStaff { get; set; }
    }
    

}