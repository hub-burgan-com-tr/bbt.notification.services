using System.Reflection;
using Elastic.Apm.Api;
using Microsoft.AspNetCore.Mvc;
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
    private readonly IConsumer _Iconsumer;

    public ConsumerController(ILogger<ConsumerController> logger, ITracer tracer, ILogHelper logHelper, IConsumer Iconsumer)
    {
        _logger = logger;
        _tracer = tracer;
        _logHelper = logHelper;
        _Iconsumer = Iconsumer;
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
            returnValue=_Iconsumer.GetUserConsumers(client, user, source);  
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
    [SwaggerOperation(
       Summary = "Insert/Update consumer ",
       Tags = new[] { "Consumer" }
   )]
    [HttpPost("/consumers/clients/{client}/sourceId/{sourceId}/postconsumer")]
    [SwaggerResponse(200, "Success, consumers is returned successfully", typeof(PostConsumerRequest))]

    public IActionResult PostConsumers(
      [FromRoute] long client,
      [FromRoute] long sourceId,
      [FromBody] PostConsumerRequest consumer)

    {
        var span = _tracer.CurrentTransaction?.StartSpan("PostConsumersSpan", "PostConsumers");
        PostConsumerResponse postConsumerResponse = new PostConsumerResponse();
        try
        {
            postConsumerResponse = _Iconsumer.PostConsumers(client,sourceId,consumer);
        }
        catch (Exception e)
        {
            span?.CaptureException(e);

            _logHelper.LogCreate(consumer, postConsumerResponse, MethodBase.GetCurrentMethod().Name, e.Message);
            return this.StatusCode(500, e.Message);
        }
        return Ok(postConsumerResponse);
    }


}
