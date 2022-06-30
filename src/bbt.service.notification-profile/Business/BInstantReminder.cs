using System.Data.SqlClient;
using System.Data;
using notification_profile.Model;


namespace Notification.Profile.Business
{
    public class BInstantReminder : IinstandReminder
    {
        private readonly IConfiguration _configuration;
        private readonly IConsumer _Iconsumer;
        private readonly IReminderDefinition _IreminderDefinition;

        public BInstantReminder(IConfiguration configuration, IConsumer IConsumer, IReminderDefinition IreminderDefinition)
        {
            _configuration = configuration;
            _Iconsumer = IConsumer;
            _IreminderDefinition = IreminderDefinition; 
        }

        public GetInstantCustomerPermissionResponse GetCustomerPermission(string customerId,string lang)
        {
            var connectionString = _configuration.GetConnectionString("ReminderConnectionString");

            // TO DO REFACTOR 
            //Dictionary<string, string> reminderDescription = new Dictionary<string, string>()
            //{
            //        { "accountMoneyEntry", "Hesabýma para geldiðinde" },
            //        { "accountMoneyExit", "Hesabýmdan para çýkýþý olduðunda" },
            //        { "checkingAccountUpperLimitExceeded", "Vadesiz hesap bakiyesi üst limite çýktýðýnda" },
            //        { "checkingAccountLowerLimitExceeded", "Vadesiz hesap bakiyesi alt limite düþtüðünde" },
            //        { "riskyFutureTransfersAndPayments", "Bakiye yetersizliði nedeniyle ödenmeme riski taþýyan ileri tarihli para transferleri ve ödemelerde" },
            //        { "savingAccountDueDateReturn", "Vadeli mevduat hesap dönüþü" },
            //        { "cardPosLimit", "Banka Kartý Alýþveriþ Limiti" },
            //        { "cardRefund", "Banka Kartý Ýade Bildirimi" },
            //        { "cardReccurring", "Banka Kartý Talimatlý Ödeme" },
            //};

            GetReminderDefinitionResponse reminderDefinitionResponse = _IreminderDefinition.GetReminderDefinitionList(lang);


            List<DbDataEntity> dbParams = new List<DbDataEntity>();
            DbDataEntity dbData = new DbDataEntity();
            dbData.parameterName = "@CUSTOMER_ID";
            dbData.value = customerId;
            dbParams.Add(dbData);
            DataTable dt = DbCalls.ExecuteDataTable(connectionString, "REM.DG_REMINDER_SELECT", dbParams);
            GetInstantCustomerPermissionResponse instantReminder = new GetInstantCustomerPermissionResponse();
            List<ReminderGet> reminders = new List<ReminderGet>();
            ReminderGet reminder;
            foreach (DataRow dr in dt.Rows)
            {
                reminder = new ReminderGet();
                
                instantReminder.showWithoutLogin = Convert.ToBoolean(dr["SHOW_WITHOUT_LOGIN"].ToString());
                reminder.amount = Convert.ToDecimal(dr["AMOUNT"].ToString());
                reminder.email = Convert.ToBoolean(dr["SEND_EMAIL"].ToString());
                reminder.sms = Convert.ToBoolean(dr["SEND_SMS"].ToString());
                reminder.mobileNotification = Convert.ToBoolean(dr["SEND_PUSHNOTIFICATION"].ToString());
                reminder.reminderType = dr["PRODUCT_CODE"].ToString();
                GetReminderDefinitionResponse reminderDefinitionResp = _IreminderDefinition.GetReminderDefinition(reminderDefinitionResponse.ReminderDefinitionList, dr["PRODUCT_CODE"].ToString());
                if (reminderDefinitionResp != null && reminderDefinitionResp.ReminderDefinitions != null) 
                {
                    reminder.reminderDescription = reminderDefinitionResp.ReminderDefinitions.Title;
                }
                reminder.hasAmountRestriction = Convert.ToBoolean(dr["HAS_AMOUNT_RESTRICTION"].ToString());
                reminders.Add(reminder);
            }
            GetUserConsumersResponse consumers = _Iconsumer.GetUserConsumers(long.Parse(customerId), long.Parse(customerId), null);

            foreach (var consumer in consumers.Consumers)
            {
                reminder = new ReminderGet();
                reminder.email = consumer.IsEmailEnabled;
                reminder.sms = consumer.IsSmsEnabled;
                reminder.mobileNotification = consumer.IsPushEnabled;
                reminders.Add(reminder);
            }
            instantReminder.reminders = reminders;
            return instantReminder;
        }


    }
}