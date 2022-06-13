using System.Data.SqlClient;
using System.Data;
using bbt.service.notification_profile.Model;


namespace bbt.service.notification_profile.Business
{
    public class BInstantReminder : IinstandReminder
    {
        private readonly IConfiguration _configuration;

        public BInstantReminder(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public GetInstantCustomerPermissionResponse GetCustomerPermission(string customerId)
        {
            var connectionString = _configuration.GetConnectionString("ReminderConnectionString");
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
               // reminder.reminderDescription = ParameterSettings.GetSettings(dr["PRODUCT_CODE"].ToString(), Helper.GetLang(headers));
                reminder.reminderType = dr["PRODUCT_CODE"].ToString();
                reminder.hasAmountRestriction = Convert.ToBoolean(dr["HAS_AMOUNT_RESTRICTION"].ToString());
                reminders.Add(reminder);
            }
            instantReminder.reminders = reminders;
            return instantReminder;
        }


    }
}