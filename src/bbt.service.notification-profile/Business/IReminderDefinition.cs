
namespace Notification.Profile.Business
{
    public interface IReminderDefinition
    {
        GetReminderDefinitionResponse GetReminderDefinitionListWithLang(string lang);

        GetReminderDefinitionResponse GetReminderDefinitionWithDefinitionCode(List<ReminderDefinition> definitionList,string definitionCode);
    }
}
