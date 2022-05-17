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
                        CustomerInformationModel customerInformationModel = await bGetCustomerInfo.GetTelephoneNumber(new GetTelephoneNumberRequestModel() { customerId = 20186224 });

                        if (customerInformationModel != null)
                        {
                            c.Phone.Number = customerInformationModel.telephoneNumber;
                            c.Phone.CountryCode = customerInformationModel.countryCode;
                            c.Phone.Prefix = customerInformationModel.cityCode;
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
            Console.WriteLine("CATCH " + e.Message);
            return new ObjectResult(null) { StatusCode = 500 };
        }

    }

}
