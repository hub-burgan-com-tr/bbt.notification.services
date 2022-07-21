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
using Notification.Profile.Business;
using Notification.Profile.Helper;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("[controller]")]
public class ConfigurationController : ControllerBase
{
    private readonly ITracer _tracer;
    private readonly ILogHelper _logHelper;
    private readonly ILogger<ConfigurationController> _logger;
    private readonly IConfigurations _Iconfigurations;

    public ConfigurationController(ILogger<ConfigurationController> logger, ITracer tracer, ILogHelper logHelper, IConfigurations configuraitons)
    {
        _logger = logger;
        _tracer = tracer;
        _logHelper = logHelper;
        _Iconfigurations = configuraitons;

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
            returnValue = _Iconfigurations.GetUsers(client);
        }
        catch (Exception e)
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
            returnValue = _Iconfigurations.GetUserConsumers(client, user);
        }
        catch (Exception e)
        {
            span?.CaptureException(e);
            var data = new
            {
                client = client,
                user = user
            };
            _logHelper.LogCreate(data, returnValue, MethodBase.GetCurrentMethod().Name, e.Message);

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
        PostUpdateResponse postUpdateResponse = new PostUpdateResponse();
        try
        {
            postUpdateResponse = _Iconfigurations.UpdateEmail(user, data);
        }
        catch (Exception e)
        {
            span?.CaptureException(e);
            var req = new
            {
                user = user,
                data = data
            };
            _logHelper.LogCreate(req, postUpdateResponse, MethodBase.GetCurrentMethod().Name, e.Message);
            return this.StatusCode(500, e.Message);
        }
        return Ok(postUpdateResponse);
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
        PostUpdateResponse postUpdateResponse = new PostUpdateResponse();
        try
        {
            postUpdateResponse = _Iconfigurations.UpdatePhone(user, data);
        }
        catch (Exception e)
        {
            span?.CaptureException(e);
            var req = new
            {
                user = user,
                data = data
            };
            _logHelper.LogCreate(req, postUpdateResponse, MethodBase.GetCurrentMethod().Name, e.Message);
            return this.StatusCode(500, e.Message);
        }
        return Ok(postUpdateResponse);
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
        PostUpdateResponse postUpdateResponse = new PostUpdateResponse();
        try
        {
            postUpdateResponse = _Iconfigurations.UpdateDevice(user, data);
        }
        catch (Exception e)
        {
            span?.CaptureException(e);
            var req = new
            {
                user = user,
                data = data
            };
            _logHelper.LogCreate(req, postUpdateResponse, MethodBase.GetCurrentMethod().Name, e.Message);
            return this.StatusCode(500, e.Message);
        }
        return Ok(postUpdateResponse);
    }


}
