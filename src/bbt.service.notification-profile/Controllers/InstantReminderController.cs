
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Swashbuckle.AspNetCore.Annotations;
using bbt.service.notification_profile.Business;

namespace bbt.service.notification_profile.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class InstantReminderController : ControllerBase
    {

        private readonly IinstandReminder _IinstandReminder;

        public InstantReminderController(IinstandReminder instantReminder)
        {
            _IinstandReminder = instantReminder;
        }




        [SwaggerOperation(
          Summary = "Returns CustomerPermission data sources",
          Tags = new[] { "InstantReminder" }
      )]
        [HttpGet("/CustomerPermission")]
        [SwaggerResponse(200, "Success, Customerpermission are returned successfully", typeof(GetInstantCustomerPermissionResponse))]
        public IActionResult CustomerPermission(string customerId)
        {
            try
            {
                GetInstantCustomerPermissionResponse getInstantCustomerPermissionResponse = _IinstandReminder.GetCustomerPermission(customerId);
                return Ok(getInstantCustomerPermissionResponse);
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.StatusCode(500, e.Message);
            }
        }

    }
}