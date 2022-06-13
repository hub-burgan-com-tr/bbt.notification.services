using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Elastic.Apm.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Notification.Profile.Helper;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("[controller]")]
public class ConfigurationController : ControllerBase
{
    private readonly ITracer _tracer;
    private readonly ILogHelper _logHelper;
    private readonly ILogger<ConfigurationController> _logger;

    public ConfigurationController(ILogger<ConfigurationController> logger, ITracer tracer,ILogHelper logHelper)
    {
        _logger = logger;
        _tracer = tracer;
        _logHelper = logHelper;
    }

    [SwaggerOperation(
              Summary = "Returns users of client. For individual clients genarally has one user -himself. ",
              Tags = new[] { "Configuration" }
          )]
    [HttpGet("/configuration/clients/{client}/users")]
    [SwaggerResponse(200, "Success, users are returned successfully", typeof(GetClientUsersResponse))]

    public IActionResult GetUsers(
        [FromRoute] long client
    )
    {
        var span = _tracer.CurrentTransaction?.StartSpan("GetUsersSpan", "GetUsers");
        GetClientUsersResponse returnValue = new GetClientUsersResponse();
        try
        {
            using (var db = new DatabaseContext())
            {
                var users = db.Consumers.Where(s => s.Client == client)
                    .Select(m => m.User)
                    .Distinct()
                    .ToList();

                returnValue.Users = users;

            }
        }
        catch(Exception e)
        {
            span?.CaptureException(e);
        
            _logHelper.LogCreate(client, returnValue, MethodBase.GetCurrentMethod().Name, e.Message);
            return this.StatusCode(500, e.Message);
        }
        return Ok(returnValue);
    }

    [SwaggerOperation(
           Summary = "Returns consumer configurations tree of user",
           Tags = new[] { "Configuration" }
       )]
    [HttpGet("/configuration/clients/{client}/users/{user}/consumer-tree")]
    [SwaggerResponse(200, "Success, consumers is returned successfully", typeof(GetConsumerTreeResponse))]

    public IActionResult GetUserConsumers(
      [FromRoute] long client,
      [FromRoute] long user)

    {
        var span = _tracer.CurrentTransaction?.StartSpan("GetUserConsumersSpan", "GetUserConsumers");
        GetConsumerTreeResponse returnValue = new GetConsumerTreeResponse();
     
        try
        {
            using (var db = new DatabaseContext())
            {
                var consumers = db.Consumers.Where(s =>
                    s.Client == client &&
                    s.User == user
                ).ToList();

                var sources = db.Sources.Select(x => x.Id);
                List<GetConsumerTreeResponse.ConfUser> confUsers = new List<GetConsumerTreeResponse.ConfUser>();


                foreach (var sourceId in sources)
                {
                    consumers = null;
                    var consumer = consumers.FirstOrDefault(x => sourceId == x.SourceId);

                    if (consumer != null)
                    {
                        confUsers.Add(new GetConsumerTreeResponse.ConfUser
                        {
                            Source = consumer.SourceId,
                            Filter = consumer.Filter,
                            IsPushEnabled = consumer.IsPushEnabled,
                            DeviceKey = consumer.DeviceKey,
                            IsSmsEnabled = consumer.IsSmsEnabled,
                            Phone = consumer.Phone,
                            IsEmailEnabled = consumer.IsEmailEnabled,
                            Email = consumer.Email
                        });


                    }

                    else
                    {
                        confUsers.Add(new GetConsumerTreeResponse.ConfUser
                        {
                            Source = sourceId,
                            Filter = null,
                            IsPushEnabled = false,
                            DeviceKey = null,
                            IsSmsEnabled = false,
                            Phone = null,
                            IsEmailEnabled = false,
                            Email = null
                        });

                    }
                }

                returnValue.Consumers = confUsers;
            }
        }
        catch(Exception e)
        {
            span?.CaptureException(e);
            var data = new { 
                client = client,
                user = user
            };
            _logHelper.LogCreate(data, returnValue, MethodBase.GetCurrentMethod().Name,e.Message);
           
            return this.StatusCode(500, e.Message);
        }

        return Ok(returnValue);
    }

    [SwaggerOperation(
           Summary = "Updates consumer configurations tree branch of user",
           Tags = new[] { "Configuration" }
       )]
    [HttpPost("/configuration/clients/{client}/users/{user}/consumer-tree")]
    [SwaggerResponse(200, "Success, consumers is returned successfully", typeof(GetUserConsumersResponse))]

    public IActionResult PostUserConsumers(
      [FromRoute] long client,
      [FromRoute] long user)

    {
        throw new NotImplementedException();
    }




    [SwaggerOperation(
                Summary = "Updates user email address in all consumers",
             Tags = new[] { "Configuration" }
         )]
    [HttpPatch("/configuration/users/{user}/update-email")]
    [SwaggerResponse(200, "Success, email addresses are updated successfully", typeof(PostUpdateResponse))]

    public IActionResult UpdateEmail(
        [FromRoute] long user,
        [FromBody] PostUpdateEmailRequest data

    )
    {
        var span = _tracer.CurrentTransaction?.StartSpan("UpdateEmailSpan", "UpdateEmail");
        int result=0;
        try
        {
            using (var db = new DatabaseContext())
            {
                result = db.Database.ExecuteSqlInterpolated($"UPDATE [Consumers] SET Email = '{data.NewEmail}' WHERE Email = '{data.OldEmail}' AND [User] = {user}");

                return Ok(new PostUpdateResponse { UpdatedRecordCount = result });
            }
        }
        catch(Exception e)
        {
            span?.CaptureException(e);
            var req = new
            {
                user = user,
                data = data
            };
            _logHelper.LogCreate(req, result, MethodBase.GetCurrentMethod().Name, e.Message);
            return this.StatusCode(500, e.Message);
        }
    }

    [SwaggerOperation(
               Summary = "Updates user phone in all consumers",
             Tags = new[] { "Configuration" }
         )]
    [HttpPatch("/configuration/users/{user}/update-phone")]
    [SwaggerResponse(200, "Success, consumer is returned successfully", typeof(PostUpdateResponse))]

    public IActionResult UpdatePhone(
        [FromRoute] long user,
        [FromBody] PostUpdatePhoneRequest data

    )
    {
        var span = _tracer.CurrentTransaction?.StartSpan("PostUpdatePhoneRequestSpan", "PostUpdatePhoneRequest");
        int result = 0;
        try
        {
            using (var db = new DatabaseContext())
            {
                 result = db.Database.ExecuteSqlInterpolated($@"UPDATE [Consumers] 
                        SET Phone_CountryCode = {data.NewPhone.CountryCode},  
                            Phone_Prefix = {data.NewPhone.Prefix},
                            Phone_Number = {data.NewPhone.Number} 
                        WHERE Phone_CountryCode = {data.OldPhone.CountryCode} AND
                              Phone_Prefix = {data.OldPhone.Prefix} AND 
                              Phone_Number = {data.OldPhone.Number} AND 
                              [User] = {user}");

                return Ok(new PostUpdateResponse { UpdatedRecordCount = result });
            }
        }
        catch(Exception e)
        {
            span?.CaptureException(e);
            var req = new
            {
                user = user,
                data = data
            };
            _logHelper.LogCreate(req, result, MethodBase.GetCurrentMethod().Name, e.Message);
            return this.StatusCode(500, e.Message);
        }

    }


    [SwaggerOperation(
              Summary = "Updates user device in all consumers",
           Tags = new[] { "Configuration" }
       )]
    [HttpPatch("/configuration/users/{user}/update-device")]
    [SwaggerResponse(200, "Success, consumer is returned successfully", typeof(PostUpdateResponse))]

    public IActionResult UpdateDevice(
      [FromRoute] long user,
        [FromBody] PostUpdateDeviceRequest data

    )
    {
        var span = _tracer.CurrentTransaction?.StartSpan("UpdateDeviceSpan", "UpdateDevice");
        int result = 0;
        try
        {
            using (var db = new DatabaseContext())
            {

                result = db.Database.ExecuteSqlInterpolated($"UPDATE [Consumers] SET DeviceKey = '{data.NewDeviceKey}' WHERE DeviceKey = '{data.OldDeviceKey}' AND [User] = {user}");
                return Ok(new PostUpdateResponse { UpdatedRecordCount = result });
            }
        }
        catch (Exception e)
        {
            span?.CaptureException(e);
            var req = new
            {
                user = user,
                data = data
            };
            _logHelper.LogCreate(req, result, MethodBase.GetCurrentMethod().Name, e.Message);
            return this.StatusCode(500, e.Message);
        }
    }


}
