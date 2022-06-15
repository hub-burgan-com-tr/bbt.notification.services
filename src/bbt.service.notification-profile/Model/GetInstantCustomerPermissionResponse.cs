
using Notification.Profile.Model.BaseResponseModel;

public class GetInstantCustomerPermissionResponse:BaseResponseModel
{
    public bool showWithoutLogin { get; set; }
    public List<ReminderGet> reminders { get; set; }

}

public class ReminderGet
{
    public string reminderType { get; set; }
    public string reminderDescription { get; set; }
    public bool sms { get; set; }
    public bool email { get; set; }
    public bool mobileNotification { get; set; }
    public decimal amount { get; set; }
    public bool hasAmountRestriction { get; set; }
}