using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using Notification.Profile.Business;
using bbt.framework.dengage.Business;
using Elastic.Apm.Api;
using Notification.Profile.Helper;
using System.Reflection;
using Notification.Profile.Enum;



//namespace Notification.Profile.Controllers;

[ApiController]
[Route("[controller]")]
public class SourceController : ControllerBase
{
    private readonly ITracer _tracer;
    private readonly ILogger<SourceController> _logger;
    private readonly ILogHelper _logHelper;
    private readonly ISource _Isource;

    public SourceController(ILogger<SourceController> logger, ITracer tracer, ILogHelper logHelper, ISource Isource)
    {
        _logger = logger;
        _tracer = tracer;
        _logHelper = logHelper;
        _Isource = Isource;
    }

    [SwaggerOperation(
              Summary = "Returns registered data sources",
              Tags = new[] { "Source" }
          )]
    [HttpGet("/sources")]
    [SwaggerResponse(200, "Success, sources are returned successfully", typeof(GetSourcesResponse))]

    public IActionResult GetSources()
    {
        List<Source> sources = null;
        GetSourcesResponse getSourcesResponse = new GetSourcesResponse();
        var span = _tracer.CurrentTransaction?.StartSpan("GetSourcesSpan", "GetSources");
        try
        {
            getSourcesResponse = _Isource.GetSources();
        }
        catch (Exception e)
        {
            span?.CaptureException(e);

            _logHelper.LogCreate(null, sources, MethodBase.GetCurrentMethod().Name, e.Message);
            return this.StatusCode(500, e.Message);
        }
        return Ok(getSourcesResponse);
    }


    [SwaggerOperation(
              Summary = "Returns registered data source",
              Tags = new[] { "Source" }
          )]
    [HttpGet("/sources/id/{id}")]
    [SwaggerResponse(200, "Success, specify id source are returned successfully", typeof(GetSourceTopicByIdResponse))]
    [SwaggerResponse(460, "Source is not found.", typeof(Guid))]

    public IActionResult GetSourceById(
      [FromRoute] int id

 )
    {
        var span = _tracer.CurrentTransaction?.StartSpan("GetSourceByIdSpan", "GetSourceById");
        GetSourceTopicByIdResponse returnValue = new GetSourceTopicByIdResponse();
        try
        {
            returnValue = _Isource.GetSourceById(id);
            if (returnValue != null && returnValue.Result == ResultEnum.Error)
            {
                span.CaptureErrorLog(new ErrorLog("Error Message( StatusCode:" + returnValue.StatusCode + " - Message:" + returnValue.MessageList[0].ToString() + ")")
                {
                    Level = "error",
                    ParamMessage = returnValue.StatusCode + " - " + returnValue.MessageList[0].ToString()
                }
                );

                return this.StatusCode(Convert.ToInt32(returnValue.StatusCode), returnValue.MessageList);
                // new ObjectResult(id) { StatusCode = 460 };
            }
            else
            {
                return Ok(returnValue);
            }
        }
        catch (Exception e)
        {
            span?.CaptureException(e);

            _logHelper.LogCreate(id, returnValue, MethodBase.GetCurrentMethod().Name, e.Message);
            return this.StatusCode(500, e.Message);
        }

    }

    [SwaggerOperation(
             Summary = "Adds new data sources",
             Tags = new[] { "Source" }
         )]
    [HttpPost("/sources")]
    [SwaggerResponse(200, "Success, sources is created successfully", typeof(void))]
    public IActionResult Post(
     [FromBody] PostSourceRequest data)
    {
        var span = _tracer.CurrentTransaction?.StartSpan("PostSpan", "Post");
        try
        {
            _Isource.Post(data);

        }
        catch (Exception e)
        {
            span?.CaptureException(e);
            _logHelper.LogCreate(data, "StatusCode:500", MethodBase.GetCurrentMethod().Name, e.Message);
            return this.StatusCode(500, e.Message);
        }

        return Ok();
    }


    [SwaggerOperation(
             Summary = "Updates existing data sources. Only not null values are going be to uptated",
             Tags = new[] { "Source" }
         )]
    [HttpPatch("/sources/{id}")]
    [SwaggerResponse(200, "Success, source is updated successfully", typeof(SourceResponseModel))]
    [SwaggerResponse(460, "Source is not found.", typeof(Guid))]
    public IActionResult Patch(
        [FromRoute] int id,
        [FromBody] PatchSourceRequest data)
    {
        var span = _tracer.CurrentTransaction?.StartSpan("PatchSourceRequestSpan", "PatchSourceRequest");
        SourceResponseModel sourceResp = new SourceResponseModel();
        try
        {
            sourceResp = _Isource.Patch(id, data);
            if (sourceResp != null && sourceResp.Result == ResultEnum.Error)
            {
                span.CaptureErrorLog(new ErrorLog("Error Message( StatusCode:" + sourceResp.StatusCode + " - Message:" + sourceResp.MessageList[0].ToString() + ")")
                {
                    Level = "error",
                    ParamMessage = sourceResp.StatusCode + " - " + sourceResp.MessageList[0].ToString()
                });
                _logHelper.LogCreate(id, sourceResp.StatusCode, MethodBase.GetCurrentMethod().Name, sourceResp.MessageList[0]);
                return this.StatusCode(Convert.ToInt32(sourceResp.StatusCode), sourceResp.MessageList);
            }

        }
        catch (Exception e)
        {
            span?.CaptureException(e);
            var req = new
            {
                id = id,
                data = data
            };
            _logHelper.LogCreate(req, "StatusCode:500", MethodBase.GetCurrentMethod().Name, e.Message);
            return this.StatusCode(500, e.Message);
        }
        return Ok();
    }


    [SwaggerOperation(
            Summary = "Deletes existing data source if only there is no referenced consumer",
            Tags = new[] { "Source" }
        )]
    [HttpDelete("/sources/{id}")]
    [SwaggerResponse(200, "Success, source is deleted successfully", typeof(SourceResponseModel))]
    [SwaggerResponse(460, "Source is not found", typeof(Guid))]
    [SwaggerResponse(461, "Source has consumer(s)", typeof(Guid))]
    public IActionResult Delete([FromRoute] int id)
    {
        var span = _tracer.CurrentTransaction?.StartSpan("DeleteSpan", "Delete");
        SourceResponseModel respModel = new SourceResponseModel();
        try
        {
            respModel = _Isource.Delete(id);
            if (respModel != null && respModel.Result == Notification.Profile.Enum.ResultEnum.Error)
            {
                span.CaptureErrorLog(new ErrorLog("Error Message( StatusCode:" + respModel.StatusCode + " - Message:" + respModel.MessageList[0].ToString() + ")")
                {
                    Level = "error",
                    ParamMessage = respModel.StatusCode + " - " + respModel.MessageList[0].ToString()
                });
                _logHelper.LogCreate(id, respModel.StatusCode, MethodBase.GetCurrentMethod().Name, respModel.MessageList[0]);
                return this.StatusCode(Convert.ToInt32(respModel.StatusCode), respModel.MessageList);
            }

        }
        catch (Exception e)
        {
            span?.CaptureException(e);
            _logHelper.LogCreate(id, StatusCodeEnum.StatusCode500.ToString(), MethodBase.GetCurrentMethod().Name, e.Message);
            return this.StatusCode(500, e.Message);
        }
        return Ok(respModel);
    }


    [SwaggerOperation(
          Summary = "Returns all consumers with filtering (if available)",
          Tags = new[] { "Source" }
      )]
    [HttpPost("/sources/consumers-by-client")]
    [SwaggerResponse(200, "Success, consumers is returned successfully", typeof(GetSourceConsumersResponse))]
    [SwaggerResponse(470, "No results were found for the given parameters", typeof(Guid))]
    public async Task<ActionResult<GetSourceConsumersResponse>> GetSourceConsumers([FromBody] GetSourceConsumersRequestBody requestModel)
    {
        GetSourceConsumersResponse returnValue = new GetSourceConsumersResponse { Consumers = new List<GetSourceConsumersResponse.Consumer>() };
        var span = _tracer.CurrentTransaction?.StartSpan("GetSourceConsumersSpan", "GetSourceConsumers");
        try
        {
            returnValue = _Isource.GetSourceConsumers(requestModel);
            if (returnValue != null && returnValue.Result == ResultEnum.Error)
            {
                span.CaptureErrorLog(new ErrorLog("Error Message( StatusCode:" + returnValue.StatusCode + " - Message:" + returnValue.MessageList[0].ToString() + ")")
                {
                    Level = "error",
                    ParamMessage = returnValue.StatusCode + " - " + returnValue.MessageList[0].ToString()
                });
                _logHelper.LogCreate(requestModel, returnValue.StatusCode, MethodBase.GetCurrentMethod().Name, returnValue.MessageList[0]);
                return this.StatusCode(Convert.ToInt32(returnValue.StatusCode), returnValue.MessageList);
            }
        }
        catch (Exception e)
        {
            span?.CaptureException(e);
            _logHelper.LogCreate(requestModel, returnValue, MethodBase.GetCurrentMethod().Name, e.Message);
            Console.WriteLine("CATCH " + e.Message);
            return new ObjectResult(null) { StatusCode = 500 };
        }
        return Ok(returnValue);

    }
}
