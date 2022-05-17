using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using bbt.framework.dengage.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Notification.Profile.Business;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("[controller]")]
public class ConsumerController : ControllerBase
{

    private readonly ILogger<ConsumerController> _logger;

    public ConsumerController(ILogger<ConsumerController> logger)
    {
        _logger = logger;
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

        using (var db = new DatabaseContext())
        {
            var consumers = db.Consumers.Where(s =>
                s.Client == client &&
                s.User == user &&
                (source == null || source.HasValue || s.SourceId == source.Value))
            .AsNoTracking();

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

        return Ok(returnValue);
    }


}
