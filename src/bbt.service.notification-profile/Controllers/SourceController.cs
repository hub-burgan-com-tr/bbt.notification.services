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



//namespace Notification.Profile.Controllers;

[ApiController]
[Route("[controller]")]
public class SourceController : ControllerBase
{
    private readonly ITracer _tracer;
    private readonly ILogger<SourceController> _logger;
    private readonly ILogHelper _logHelper;

    public SourceController(ILogger<SourceController> logger, ITracer tracer, ILogHelper logHelper)
    {
        _logger = logger;
        _tracer = tracer;
        _logHelper = logHelper;
    }

    [SwaggerOperation(
              Summary = "Returns registered data sources",
              Tags = new[] { "Source" }
          )]
    [HttpGet("/sources")]
    [SwaggerResponse(200, "Success, sources are returned successfully", typeof(GetSourcesResponse))]

    public IActionResult GetSources()
    {
        List<Source> sources=null;
        var span = _tracer.CurrentTransaction?.StartSpan("GetSourcesSpan", "GetSources");
        try
        {
            using (var db = new DatabaseContext())
            {
                sources = db.Sources
                    .Include(s => s.Parameters)
                    .Include(s => s.Children)
                    .ToList();
            }


            return Ok(new GetSourcesResponse
            {
                Sources = sources.Where(s => s.ParentId == null).Select(s => BuildSource(s)).ToList()
            }
            );

            GetSourcesResponse.Source BuildSource(Source s)
            {
                return new GetSourcesResponse.Source
                {
                    Id = s.Id,
                    Title = new GetSourcesResponse.Source.TitleLabel { EN = s.Title_EN, TR = s.Title_TR },
                    Children = s.Children.Select(c => BuildSource(c)).ToList(),
                    Parameters = s.Parameters.Select(p => new GetSourcesResponse.Source.SourceParameter
                    {
                        JsonPath = p.JsonPath,
                        Type = p.Type,
                        Title = new GetSourcesResponse.Source.TitleLabel { EN = p.Title_EN, TR = p.Title_TR },
                    }).ToList(),
                    Topic = s.Topic,
                    DisplayType = s.DisplayType,
                    ClientIdJsonPath = s.ClientIdJsonPath,
                    ApiKey = s.ApiKey,
                    Secret = s.Secret,
                    PushServiceReference = s.PushServiceReference,
                    SmsServiceReference = s.SmsServiceReference,
                    EmailServiceReference = s.EmailServiceReference
                };

            }
        }
        catch(Exception e)
        {
            span?.CaptureException(e);
          
            _logHelper.LogCreate(null, sources, MethodBase.GetCurrentMethod().Name, e.Message);
            return this.StatusCode(500, e.Message);
        }
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
          

            Source source = null;
            List<SourceServicesUrl> servicesUrls = null;
            using (var db = new DatabaseContext())
            {
                source = db.Sources.Where(s => s.Id == id).FirstOrDefault();
                servicesUrls = db.SourceServices.Where(s => id == s.SourceId).Select(x => new SourceServicesUrl
                {
                    Id = x.Id,
                    ServiceUrl = x.ServiceUrl
                }).ToList();
            }

            if (source == null)
                return new ObjectResult(id) { StatusCode = 460 };



            SourceServicesUrl sourceServicesUrl = new SourceServicesUrl();

            returnValue.Id = source.Id;
            returnValue.Topic = source.Topic;
            returnValue.SmsServiceReference = source.SmsServiceReference;
            returnValue.EmailServiceReference = source.EmailServiceReference;
            returnValue.PushServiceReference = source.PushServiceReference;
            returnValue.Title_TR = source.Title_TR;
            returnValue.Title_EN = source.Title_EN;
            returnValue.ParentId = source.ParentId;
            returnValue.DisplayType = source.DisplayType;
            returnValue.ApiKey = source.ApiKey;
            returnValue.Secret = source.Secret;
            returnValue.ClientIdJsonPath = source.ClientIdJsonPath;
            returnValue.KafkaUrl = source.KafkaUrl;
            returnValue.ServiceUrlList = servicesUrls;
            return Ok(returnValue);
        }
        catch(Exception e)
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
            using (var db = new DatabaseContext())
            {
                db.Add(new Source
                {
                    Id = data.Id,
                    //Title = data.Title,
                    Topic = data.Topic,
                    ApiKey = data.ApiKey,
                    Secret = data.Secret,
                    PushServiceReference = data.PushServiceReference,
                    SmsServiceReference = data.SmsServiceReference,
                    EmailServiceReference = data.EmailServiceReference
                });

                db.SaveChanges();
            }

        }
        catch(Exception e)
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
    [SwaggerResponse(200, "Success, source is updated successfully", typeof(void))]
    [SwaggerResponse(460, "Source is not found.", typeof(Guid))]
    public IActionResult Patch(
        [FromRoute] int id,
        [FromBody] PatchSourceRequest data)
    {
        var span = _tracer.CurrentTransaction?.StartSpan("PatchSourceRequestSpan", "PatchSourceRequest");
        try
        {
            using (var db = new DatabaseContext())
            {
                var source = db.Sources.FirstOrDefault(s => s.Id == id);

                if (source == null)
                    return new ObjectResult(id) { StatusCode = 460 };

                //if (data.Title != null) source.Title = data.Title;
                if (data.Topic != null) source.Topic = data.Topic;
                if (data.ApiKey != null) source.ApiKey = data.ApiKey;
                if (data.Secret != null) source.Secret = data.Secret;
                if (data.PushServiceReference != null) source.PushServiceReference = data.PushServiceReference;
                if (data.SmsServiceReference != null) source.SmsServiceReference = data.SmsServiceReference;
                if (data.EmailServiceReference != null) source.EmailServiceReference = data.EmailServiceReference;

                db.SaveChanges();
            }

        }
        catch(Exception e)
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
    [SwaggerResponse(200, "Success, source is deleted successfully", typeof(void))]
    [SwaggerResponse(460, "Source is not found", typeof(Guid))]
    [SwaggerResponse(461, "Source has consumer(s)", typeof(Guid))]
    public IActionResult Delete([FromRoute] int id)
    {
        var span = _tracer.CurrentTransaction?.StartSpan("DeleteSpan", "Delete");
        try
        {
            using (var db = new DatabaseContext())
            {
                var source = db.Sources.FirstOrDefault(s => s.Id == id);

                if (source == null)
                    return new ObjectResult(id) { StatusCode = 460 };

                var references = db.Consumers.FirstOrDefault(c => c.SourceId == id);

                if (references != null)
                    return new ObjectResult(id) { StatusCode = 461 };

                db.Remove(source);
                db.SaveChanges();
            }
        }
        catch (Exception e)
        {
            span?.CaptureException(e);
            _logHelper.LogCreate(id, "StatusCode:500", MethodBase.GetCurrentMethod().Name, e.Message);
            return this.StatusCode(500, e.Message);
        }
        return Ok();
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

            dynamic message = null;
            List<Consumer> consumers = null;
            using (var db = new DatabaseContext())
            {
                // 0 nolu musteri generic musteri olarak kabul ediliyor. Banka kullanicilarin ozel durumlarda subscription olusturmalari icin kullanilacak.
                consumers = db.Consumers.Where(s => (s.Client == requestModel.client || s.Client == 0) && s.SourceId == requestModel.sourceid).ToList();
            }

            if (consumers == null || consumers.Count() < 1)
                return new ObjectResult(consumers) { StatusCode = 470 };
            // Eger filtre yoksa bosu bosuna deserialize etme             
            if (consumers.Count>1 &&consumers.FirstOrDefault(c => c.Client == 0)!=null)
            {
                consumers.Remove(consumers.FirstOrDefault(c => c.Client == 0));
            }
            if (consumers.Any(c => c.Filter != null) && requestModel.jsonData is not null)
            {
                requestModel.jsonData = requestModel.jsonData.Replace(@"\", "");
                message = JsonConvert.DeserializeObject(requestModel.jsonData);
            }

            consumers.ForEach(async c =>
            {
                bool canSend = true; // eger filtre yoksa gonderim sekteye ugramasin.

                if (c.Filter != null && requestModel.jsonData is not null)
                {
                    canSend = Extensions.Evaluate(c.Filter, message);
                }

                if (canSend)
                {
                    if (c.Client == 0)
                    {
                        BGetCustomerInfo bGetCustomerInfo = new BGetCustomerInfo(null);
                        CustomerInformationModel customerInformationModel = await bGetCustomerInfo.GetTelephoneNumber(new GetTelephoneNumberRequestModel() { name = requestModel.client.ToString() });

                        if (customerInformationModel != null)
                        {
                            c.Phone=new Phone();
                            c.Phone.Number = Convert.ToInt32(customerInformationModel.customerList[0].gsmPhone.number) ;
                            c.Phone.CountryCode = Convert.ToInt32(customerInformationModel.customerList[0].gsmPhone.country);
                            c.Phone.Prefix = Convert.ToInt32(customerInformationModel.customerList[0].gsmPhone.prefix);
                        }
                    }
                    returnValue.Consumers.Add(new GetSourceConsumersResponse.Consumer
                    {
                        Id = c.Id,
                        Client = c.Client,
                        User = c.User,
                        IsPushEnabled = c.IsPushEnabled,
                        DeviceKey = c.DeviceKey,
                        Filter = c.Filter,
                        IsSmsEnabled = c.IsSmsEnabled,
                        Phone = c.Phone,
                        IsEmailEnabled = c.IsEmailEnabled,
                        Email = c.Email
                    });
                }
            });
            return Ok(returnValue);
        }
        catch (Exception e)
        {
            span?.CaptureException(e);
            _logHelper.LogCreate(requestModel, returnValue, MethodBase.GetCurrentMethod().Name, e.Message);
            Console.WriteLine("CATCH " + e.Message);
            return new ObjectResult(null) { StatusCode = 500 };
        }

    }



}
