
using Notification.Profile.Model.BaseResponseModel;

public class GetReminderDefinitionResponse : BaseResponseModel
{
    public List<ReminderDefinition> ReminderDefinitionList { get; set; }

    public ReminderDefinition ReminderDefinitions { get; set; }
}

