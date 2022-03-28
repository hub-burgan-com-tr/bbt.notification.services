using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

//namespace Notification.Profile.Controllers;

[ApiController]
[Route("[controller]")]
public class SourceController : ControllerBase
{

    private readonly ILogger<SourceController> _logger;

    public SourceController(ILogger<SourceController> logger)
    {
        _logger = logger;
    }

    [SwaggerOperation(
              Summary = "Returns registered data sources",
              Tags = new[] { "Source" }
          )]
    [HttpGet("/sources")]
    [SwaggerResponse(200, "Success, sources are returned successfully", typeof(GetSourcesResponse))]

    public IActionResult GetSources()
    {
        List<Source> sources;

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
        GetSourceTopicByIdResponse returnValue = new GetSourceTopicByIdResponse();

        using (var db = new DatabaseContext())
        {
            var source = db.Sources.Where(s =>
                s.Id == id).FirstOrDefault();

            if (source == null)
            return new ObjectResult(id) { StatusCode = 460 };
            
            var sourceService = db.SourceServices.Where(s=> id == s.SourceId).Select(x => new SourceServicesUrl{
                Id=x.Id,
                ServiceUrl=x.ServiceUrl
            }).ToList();
            
            returnValue.Id=source.Id;
            returnValue.Topic=source.Topic;
            returnValue.SmsServiceReference=source.SmsServiceReference;
            returnValue.EmailServiceReference=source.EmailServiceReference;
            returnValue.PushServiceReference=source.PushServiceReference;
            returnValue.Title_TR=source.Title_TR;
            returnValue.Title_EN=source.Title_EN;
            returnValue.ParentId=source.ParentId;
            returnValue.DisplayType=source.DisplayType;
            returnValue.ApiKey=source.ApiKey;
            returnValue.Secret=source.Secret;
            returnValue.ClientIdJsonPath=source.ClientIdJsonPath;
            returnValue.KafkaUrl=source.KafkaUrl;
            returnValue.ServiceUrlList=sourceService;
            
        }

        return Ok(returnValue);
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

        return Ok();
    }


    [SwaggerOperation(
         Summary = "Returns all consumers with filtering (if available)",
         Tags = new[] { "Source" }
     )]
    [HttpGet("/sources/{id}/consumers-by-client/{client}")]
    [SwaggerResponse(200, "Success, consumers is returned successfully", typeof(GetSourceConsumersResponse))]
    [SwaggerResponse(470, "No results were found for the given parameters", typeof(Guid))]
    public IActionResult GetSourceConsumers(
    [FromRoute] int id,
    [FromRoute] long client,
    [DefaultValue("{   \"data\": {     \"amount\": 600,     \"iban\":\"TR1234567\",     \"name\": {       \"first\": \"ugur\",       \"last\": \"karatas\"     }   } }")]
    [FromQuery] string jsonData
)
    {
        GetSourceConsumersResponse returnValue = new GetSourceConsumersResponse { Consumers = new List<GetSourceConsumersResponse.Consumer>() };

        dynamic message = null;

        using (var db = new DatabaseContext())
        {
            // 0 nolu musteri generic musteri olarak kabul ediliyor. Banka kullanicilarin ozel durumlarda subscription olusturmalari icin kullanilacak.
            var consumers = db.Consumers.Where(s => (s.Client == client || s.Client == 0) && s.SourceId == id).ToList();

            if (consumers.Count == 0)
               return new ObjectResult(consumers) { StatusCode = 470 };

            // Eger filtre yoksa bosu bosuna deserialize etme
            if (consumers.Any(c => c.Filter != null))
            {
                message = JsonConvert.DeserializeObject(jsonData);
            }

            consumers.ForEach(c =>
            {
                bool canSend = true; // eger filtre yoksa gonderim sekteye ugramasin.

                if (c.Filter != null)
                {
                    canSend = Extensions.Evaluate(c.Filter, message);
                }

                if (canSend)

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
            });
        }

        return Ok(returnValue);
    }


}
