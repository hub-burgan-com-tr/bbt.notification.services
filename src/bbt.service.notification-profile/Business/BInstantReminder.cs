using System.Data.SqlClient;
using System.Data;
using notification_profile.Model;

using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using Notification.Profile.Model;

namespace Notification.Profile.Business
{
    public class BInstantReminder : IinstandReminder
    {
        private readonly IConfiguration _configuration;
        private readonly IConsumer _Iconsumer;
        private readonly IReminderDefinition _IreminderDefinition;
        private readonly IDistributedCache _cache;

        public BInstantReminder(IConfiguration configuration, IConsumer IConsumer, IReminderDefinition IreminderDefinition, IDistributedCache cache)
        {
            _configuration = configuration;
            _Iconsumer = IConsumer;
            _IreminderDefinition = IreminderDefinition;
            _cache = cache;
        }

        public async Task<GetInstantCustomerPermissionResponse> GetCustomerPermission(string customerId, string lang)
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
            GetInstantCustomerPermissionResponse instantReminder = new GetInstantCustomerPermissionResponse();
            List<ReminderDefinition> reminderDefinitionList = new List<ReminderDefinition>();
            GetReminderDefinitionResponse reminderDefinitionResponse = new GetReminderDefinitionResponse();

            var cachedList = await _cache.GetAsync("redis");
            if (cachedList != null && !string.IsNullOrEmpty(System.Text.Encoding.UTF8.GetString(cachedList)))
            {
                reminderDefinitionList = JsonConvert.DeserializeObject<List<ReminderDefinition>>(System.Text.Encoding.UTF8.GetString(cachedList));
            }
            else
            {
                reminderDefinitionResponse = _IreminderDefinition.GetReminderDefinitionList(lang);
                reminderDefinitionList = reminderDefinitionResponse.ReminderDefinitionList;
                await _cache.SetAsync("redis", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(reminderDefinitionList)),
                new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(1)
                });
            }


            List<DbDataEntity> dbParams = new List<DbDataEntity>();
            DbDataEntity dbData = new DbDataEntity();
            dbData.parameterName = "@CUSTOMER_ID";
            dbData.value = customerId;
            dbParams.Add(dbData);
            DataTableResponseModel responseModel = new DataTableResponseModel();
            responseModel = DbCalls.ExecuteDataTable(connectionString, "REM.DG_REMINDER_SELECT", dbParams);
            //Müþteri izinlerini çekiyor eski sistemden
            if (responseModel.Result != Enum.ResultEnum.Error)
            {
                DataTable dt = responseModel.DataTable;

                ReminderGet reminder;

                List<ReminderGet> reminders = new List<ReminderGet>();
              
                foreach (DataRow dr in dt.Rows)
                {
                    reminder = new ReminderGet();

                    instantReminder.showWithoutLogin = Convert.ToBoolean(dr["SHOW_WITHOUT_LOGIN"].ToString());
                    reminder.amount = Convert.ToDecimal(dr["AMOUNT"].ToString());
                    reminder.email = Convert.ToBoolean(dr["SEND_EMAIL"].ToString());
                    reminder.sms = Convert.ToBoolean(dr["SEND_SMS"].ToString());
                    reminder.mobileNotification = Convert.ToBoolean(dr["SEND_PUSHNOTIFICATION"].ToString());
                    reminder.reminderType = dr["PRODUCT_CODE"].ToString();
                    GetReminderDefinitionResponse reminderDefinitionResp = _IreminderDefinition.GetReminderDefinition(reminderDefinitionList, dr["PRODUCT_CODE"].ToString());
                    if (reminderDefinitionResp != null && reminderDefinitionResp.ReminderDefinitions != null)
                    {
                        reminder.reminderDescription = reminderDefinitionResp.ReminderDefinitions.Title;
                    }
                    reminder.hasAmountRestriction = Convert.ToBoolean(dr["HAS_AMOUNT_RESTRICTION"].ToString());
                    reminders.Add(reminder);
                }
                GetUserConsumersResponse consumers = _Iconsumer.GetUserConsumers(long.Parse(customerId), long.Parse(customerId), null);//buraya reminderDescription gönderecek miyiz

                foreach (var consumer in consumers.Consumers)
                {
                    reminder = new ReminderGet();
                    reminder.email = consumer.IsEmailEnabled;
                    reminder.sms = consumer.IsSmsEnabled;
                    reminder.mobileNotification = consumer.IsPushEnabled;
                    reminder.reminderType = consumer.DefinitionCode;
                    reminder.amount = 0;//Karar verilecek
                    reminder.hasAmountRestriction = true;//Karar verilecek
                   
                    GetReminderDefinitionResponse reminderDefinitionRespNew = _IreminderDefinition.GetReminderDefinition(reminderDefinitionList, consumer.DefinitionCode);
                    if (reminderDefinitionRespNew != null && reminderDefinitionRespNew.ReminderDefinitions != null)
                    {
                        reminder.reminderDescription = reminderDefinitionRespNew.ReminderDefinitions.Title;
                    }
                    reminders.Add(reminder);
                }
                instantReminder.reminders = reminders;
            }
            else
            {
                instantReminder.Result = Enum.ResultEnum.Error;
                instantReminder.MessageList = responseModel.MessageList;
            }
          
            return instantReminder;
        }


    }
}
