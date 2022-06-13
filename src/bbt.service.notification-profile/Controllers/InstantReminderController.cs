
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Swashbuckle.AspNetCore.Annotations;
using bbt.service.notification_profile.Business;
using Elastic.Apm.Api;
using Notification.Profile.Helper;
using System.Reflection;

namespace bbt.service.notification_profile.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class InstantReminderController : ControllerBase
    {

        private readonly IinstandReminder _IinstandReminder;
        private readonly ITracer _tracer;
        private readonly ILogHelper _logHelper;

        public InstantReminderController(IinstandReminder instantReminder, ITracer tracer, ILogHelper logHelper)
        {
            _IinstandReminder = instantReminder;
            _tracer = tracer;
            _logHelper = logHelper; 
        }




        [SwaggerOperation(
          Summary = "Returns CustomerPermission data sources",
          Tags = new[] { "InstantReminder" }
      )]
        [HttpGet("/CustomerPermission")]
        [SwaggerResponse(200, "Success, Customerpermission are returned successfully", typeof(GetInstantCustomerPermissionResponse))]
        public IActionResult CustomerPermission(string customerId)
        {
            var span = _tracer.CurrentTransaction?.StartSpan("CustomerPermissionSpan", "CustomerPermission");
            GetInstantCustomerPermissionResponse getInstantCustomerPermissionResponse = new GetInstantCustomerPermissionResponse();
            try
            {
                getInstantCustomerPermissionResponse = _IinstandReminder.GetCustomerPermission(customerId);
                return Ok(getInstantCustomerPermissionResponse);
            }

            catch (Exception e)
            {
                span?.CaptureException(e);
              
                _logHelper.LogCreate(customerId, getInstantCustomerPermissionResponse, MethodBase.GetCurrentMethod().Name, e.Message);
                Console.WriteLine(e.Message);
                return this.StatusCode(500, e.Message);
            }
        }

    }
}