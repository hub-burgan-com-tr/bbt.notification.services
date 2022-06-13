using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using bbt.framework.dengage.Business;
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
public class ConsumerController : ControllerBase
{

    private readonly ILogger<ConsumerController> _logger;
    private readonly ITracer _tracer;
    private readonly ILogHelper _logHelper;

    public ConsumerController(ILogger<ConsumerController> logger, ITracer tracer, ILogHelper logHelper)
    {
        _logger = logger;
        _tracer = tracer;
        _logHelper = logHelper;
    }

    [SwaggerOperation(
           Summary = "Returns consumer configurations of user",
           Tags = new[] { "Consumer" }
       )]
    [HttpGet("/consumers/clients/{client}/users/{user}")]
    [SwaggerResponse(200, "Success, consumers is returned successfully", typeof(GetUserConsumersResponse))]

    public IActionResult GetUserConsumers(
      [FromRoute] long client,
      [FromRoute] long user,
      [FromQuery] int? source
 )
    {
        GetUserConsumersResponse returnValue = new GetUserConsumersResponse();
        var span = _tracer.CurrentTransaction?.StartSpan("GetUserConsumersSpan", "GetUserConsumers");
        try
        {
            using (var db = new DatabaseContext())
            {
                var consumers = db.Consumers.Where(s =>
                    s.Client == client &&
                    s.User == user &&
                    (source == null || source.HasValue || s.SourceId == source.Value))
                .AsNoTracking();
                consumers = null;
                returnValue.Consumers = consumers.Select(c =>
                    new GetUserConsumersResponse.Consumer
                    {
                        Source = c.SourceId,
                        Filter = c.Filter,
                        IsPushEnabled = c.IsPushEnabled,
                        DeviceKey = c.DeviceKey,
                        IsSmsEnabled = c.IsSmsEnabled,
                        Phone = c.Phone,
                        IsEmailEnabled = c.IsEmailEnabled,
                        Email = c.Email
                    }

                ).ToList();
            }
        }
        catch(Exception e)
        {
            span?.CaptureException(e);
            var data = new
            {
                client = client,
                user = user,
                source=source
            };
            _logHelper.LogCreate(data, returnValue, MethodBase.GetCurrentMethod().Name, e.Message);
            return this.StatusCode(500, e.Message);
        }

        return Ok(returnValue);
    }


}
