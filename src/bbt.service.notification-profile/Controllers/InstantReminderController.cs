
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Swashbuckle.AspNetCore.Annotations;
using Notification.Profile.Business;
using Elastic.Apm.Api;
using Notification.Profile.Helper;
using System.Reflection;
using Notification.Profile.Enum;
using Microsoft.Extensions.Caching.Distributed;
using Notification.Profile.Model;

namespace bbt.service.notification_profile.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class InstantReminderController : ControllerBase
    {

        private readonly IinstandReminder _IinstandReminder;
        private readonly ITracer _tracer;
        private readonly ILogHelper _logHelper;
        private readonly IDistributedCache _cache;

        public InstantReminderController(IinstandReminder instantReminder, ITracer tracer, ILogHelper logHelper, IDistributedCache cache)
        {
            _IinstandReminder = instantReminder;
            _tracer = tracer;
            _logHelper = logHelper;
            _cache = cache;
        }
        [SwaggerOperation(
          Summary = "Returns CustomerPermission data sources",
          Tags = new[] { "InstantReminder" }
      )]
        [HttpGet("/CustomerPermission/{customerId}")]
        [SwaggerResponse(200, "Success, Customerpermission are returned successfully", typeof(GetInstantCustomerPermissionResponse))]
        public IActionResult CustomerPermission(string customerId)
        {
            var span = _tracer.CurrentTransaction?.StartSpan("CustomerPermissionSpan", "CustomerPermission");
            if (!Request.Headers.TryGetValue("lang", out var lang))
            {
                lang = EnumHelper.GetDescription<LanguageEnum>(LanguageEnum.TR);
            }


            GetInstantCustomerPermissionResponse getInstantCustomerPermissionResponse = new GetInstantCustomerPermissionResponse();
            try
            {
                getInstantCustomerPermissionResponse = _IinstandReminder.GetCustomerPermission(customerId, lang).Result;
                if (getInstantCustomerPermissionResponse != null && getInstantCustomerPermissionResponse.Result == ResultEnum.Error)
                {
                    _logHelper.LogCreate(customerId, getInstantCustomerPermissionResponse, MethodBase.GetCurrentMethod().Name, getInstantCustomerPermissionResponse.MessageList[0]);
                }
                else
                {
                    return Ok(getInstantCustomerPermissionResponse);
                }
            }

            catch (Exception e)
            {
                span?.CaptureException(e);

                _logHelper.LogCreate(customerId, getInstantCustomerPermissionResponse, MethodBase.GetCurrentMethod().Name, e.Message);
                Console.WriteLine(e.Message);
                return this.StatusCode(500, e.Message);
            }
            return Ok(getInstantCustomerPermissionResponse);
        }




        [SwaggerOperation(Summary = "Insert CustomerPermission DataSource",
           Tags = new[] { "InstantReminder" }
       )]
        [HttpPost("/CustomerPermission/{customerId}")]
        [SwaggerResponse(200, "Success, Customerpermission are insert", typeof(PostInstantCustomerPermissionRequest))]

        public IActionResult PostCustomerPermission(string customerId,[FromBody] PostInstantCustomerPermissionRequest request)

        {
            var span = _tracer.CurrentTransaction?.StartSpan("PostInstantReminderSpan", "PostInstantReminder");
            PostInstantCustomerPermissionResponse postConsumerResponse = new PostInstantCustomerPermissionResponse();
            try
            {
                postConsumerResponse = _IinstandReminder.PostCustomerPermission(customerId, request).Result;
                if (postConsumerResponse != null && postConsumerResponse.Result == ResultEnum.Error)
                {
                    _logHelper.LogCreate(request.reminders[0], postConsumerResponse, MethodBase.GetCurrentMethod().Name, postConsumerResponse.MessageList[0]);
                    return this.StatusCode(500, postConsumerResponse.MessageList[0]);
                }
                else
                {
                    return Ok(postConsumerResponse);
                }
            }
            catch (Exception e)
            {
                span?.CaptureException(e);
                _logHelper.LogCreate(customerId, postConsumerResponse, MethodBase.GetCurrentMethod().Name, e.Message);
                return this.StatusCode(500, e.Message);
            }
            return Ok(postConsumerResponse);

        }


    }


}