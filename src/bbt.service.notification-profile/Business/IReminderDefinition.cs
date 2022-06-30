
namespace Notification.Profile.Business
{
    public interface IReminderDefinition
    {
        GetReminderDefinitionResponse GetReminderDefinitionList(string lang);

        GetReminderDefinitionResponse GetReminderDefinition(List<ReminderDefinition> definitionList,string definitionCode);
    }
}
