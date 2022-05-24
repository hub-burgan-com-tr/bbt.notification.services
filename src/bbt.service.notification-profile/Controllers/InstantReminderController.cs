
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

using Swashbuckle.AspNetCore.Annotations;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web;



namespace bbt.service.notification_profile.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InstantReminderController : ControllerBase
    {

        [SwaggerOperation(
          Summary = "Returns CustomerPermission data sources",
          Tags = new[] { "InstantReminder" }
      )]
        [HttpGet("/CustomerPermission")]
        [SwaggerResponse(200, "Success, Customerpermission are returned successfully", typeof(GetCustomerPermissionResponse))]
        public IActionResult CustomerPermission(long customerId)
        {
            return null;
        }

    }
}