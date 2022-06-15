

using Notification.Profile.Model.BaseResponseModel;

public class GetSourceTopicByIdResponse: BaseResponseModel
{

        public int Id { get; set; }
        public string Topic { get; set; }
        public string PushServiceReference { get; set; }
        public string SmsServiceReference { get; set; }
        public string EmailServiceReference { get; set; }
        public string Title_TR { get; set; }
        public string Title_EN { get; set; }
        public int? ParentId { get; set; }
        public SourceDisplayType DisplayType { get; set; }
        public string ApiKey { get; set; }
        public string Secret{ get; set; }
        public string ClientIdJsonPath { get; set; }
        public string KafkaUrl { get; set; }
        public List<SourceServicesUrl> ServiceUrlList{ get; set; }
        
        
}


//TODO: Display diger modellerede yansimali
