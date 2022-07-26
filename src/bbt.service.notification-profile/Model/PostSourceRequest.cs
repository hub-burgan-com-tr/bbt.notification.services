

public class PostSourceRequest
{
  //  public int Id { get; set; }
    // public string Title { get; set; }
    public string Title_TR { get; set; }
    public string Title_EN { get; set; }
    public string Topic { get; set; }
    public string ApiKey { get; set; }
    public string Secret { get; set; }
    public SourceDisplayType DisplayType { get; set; }
    public string PushServiceReference { get; set; }
    public string SmsServiceReference { get; set; }
    public string EmailServiceReference { get; set; }
    public string KafkaUrl { get; set; }
    public string KafkaCertificate { get; set; }
    public string ClientIdJsonPath { get; set; }

    public int? ParentId { get; set; }

}
