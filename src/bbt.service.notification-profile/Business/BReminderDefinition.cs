using Microsoft.EntityFrameworkCore;

namespace Notification.Profile.Business
{
    public class BReminderDefinition : IReminderDefinition
    {
        private readonly IConfiguration _configuration;

        public BReminderDefinition(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public GetReminderDefinitionResponse GetReminderDefinitionListWithLang(string lang)
        {
            GetReminderDefinitionResponse returnValue = new GetReminderDefinitionResponse();
            List<ReminderDefinition> reminderDefinitionList = new List<ReminderDefinition>();
            using (var db = new DatabaseContext())
            {
                reminderDefinitionList = db.ReminderDefinitions.Where(s =>s.Language==lang).ToList();
                returnValue.ReminderDefinitionList = reminderDefinitionList;
            }
            return returnValue;
        }

        public GetReminderDefinitionResponse GetReminderDefinitionWithDefinitionCode(List<ReminderDefinition> definitionList,string definitionCode)
        {
            GetReminderDefinitionResponse returnValue = new GetReminderDefinitionResponse();
            if(definitionList!=null && !String.IsNullOrEmpty(definitionCode))
            returnValue.ReminderDefinitions= definitionList.FirstOrDefault(x => x.DefinitionCode == definitionCode);
            return returnValue;
        }
    }
}

