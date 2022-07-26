using bbt.framework.dengage.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Notification.Profile.Enum;
using Notification.Profile.Helper;

namespace Notification.Profile.Business
{
    public class BSource : ISource
    {
        private readonly IConfiguration _configuration;

        public BSource(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SourceResponseModel Delete(int id)
        {
            var returnValue = new SourceResponseModel();
            using (var db = new DatabaseContext())
            {
                var source = db.Sources.FirstOrDefault(s => s.Id == id);
                if (source == null)
                {
                    returnValue.StatusCode = EnumHelper.GetDescription<StatusCodeEnum>(StatusCodeEnum.StatusCode460); 
                    returnValue.MessageList.Add(StructStatusCode.StatusCode460.ToString());
                    returnValue.Result = ResultEnum.Error;
                    return returnValue;
                    //  return new ObjectResult(id) { StatusCode = 460 };
                }

                var references = db.Consumers.FirstOrDefault(c => c.SourceId == id);
                if (references != null)
                {
                    returnValue.StatusCode = EnumHelper.GetDescription<StatusCodeEnum>(StatusCodeEnum.StatusCode461);
                    returnValue.MessageList.Add(StructStatusCode.StatusCode461.ToString());
                    returnValue.Result = ResultEnum.Error;
                    return returnValue;
                }

                db.Remove(source);
                db.SaveChanges();
                returnValue.Result = ResultEnum.Success;
            }
            return returnValue;
        }

        public GetSourceTopicByIdResponse GetSourceById(int id)
        {
            GetSourceTopicByIdResponse returnValue = new GetSourceTopicByIdResponse();
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
            {
                returnValue.StatusCode = EnumHelper.GetDescription<StatusCodeEnum>(StatusCodeEnum.StatusCode460); 
                returnValue.MessageList.Add(StructStatusCode.StatusCode460.ToString());
                returnValue.Result = ResultEnum.Error;
                return returnValue;
            }

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
            returnValue.KafkaCertificate = source.KafkaCertificate;
            returnValue.ServiceUrlList = servicesUrls;

            return returnValue;
        }

        public GetSourceConsumersResponse GetSourceConsumers(GetSourceConsumersRequestBody requestModel)
        {
            GetSourceConsumersResponse returnValue = new GetSourceConsumersResponse { Consumers = new List<GetSourceConsumersResponse.Consumer>() };
            dynamic message = null;
            List<Consumer> consumers = null;
            using (var db = new DatabaseContext())
            {
                // 0 nolu musteri generic musteri olarak kabul ediliyor. Banka kullanicilarin ozel durumlarda subscription olusturmalari icin kullanilacak.
                consumers = db.Consumers.Where(s => (s.Client == requestModel.client || s.Client == 0) && s.SourceId == requestModel.sourceid).ToList();
            }
            if (consumers == null || consumers.Count() < 1)
            {
                returnValue.StatusCode = EnumHelper.GetDescription<StatusCodeEnum>(StatusCodeEnum.StatusCode470);
                returnValue.MessageList.Add(StructStatusCode.StatusCode470.ToString());
                returnValue.Result = ResultEnum.Error;
                return returnValue;
            }


            // Eger filtre yoksa bosu bosuna deserialize etme             
            if (consumers.Count > 1 && consumers.FirstOrDefault(c => c.Client == 0) != null)
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
                            c.Phone = new Phone();
                            c.Phone.Number = Convert.ToInt32(customerInformationModel.customerList[0].gsmPhone.number);
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
            return returnValue;
        }

        public GetSourcesResponse GetSources()
        {
            List<Source> sources = null;
            using (var db = new DatabaseContext())
            {
                sources = db.Sources
                    .Include(s => s.Parameters)
                    .Include(s => s.Children)
                    .ToList();
            }


            return new GetSourcesResponse
            {
                Sources = sources.Where(s => s.ParentId == null).Select(s => BuildSource(s)).ToList()
            };


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
                    EmailServiceReference = s.EmailServiceReference,
                    KafkaCertificate=s.KafkaCertificate,
                    KafkaUrl=s.KafkaUrl
                    
                    
                };

            }
        }

        public SourceResponseModel Patch(int id, PatchSourceRequest data)
        {
            SourceResponseModel sourceResp = new SourceResponseModel();
            using (var db = new DatabaseContext())
            {
               var  source = db.Sources.FirstOrDefault(s => s.Id == id);
                if (source == null)
                {

                    sourceResp.StatusCode = EnumHelper.GetDescription<StatusCodeEnum>(StatusCodeEnum.StatusCode470);
                    sourceResp.MessageList.Add(StructStatusCode.StatusCode470.ToString());
                    sourceResp.Result = ResultEnum.Error;
                    return sourceResp;
                    
                }

                //if (data.Title != null) source.Title = data.Title;
                if (data.Topic != null) source.Topic = data.Topic;
                if (data.ApiKey != null) source.ApiKey = data.ApiKey;
                if (data.Secret != null) source.Secret = data.Secret;
                if (data.PushServiceReference != null) source.PushServiceReference = data.PushServiceReference;
                if (data.SmsServiceReference != null) source.SmsServiceReference = data.SmsServiceReference;
                if (data.EmailServiceReference != null) source.EmailServiceReference = data.EmailServiceReference;
                if (data.KafkaUrl != null) source.KafkaUrl = data.KafkaUrl;
                if (data.KafkaCertificate != null) source.KafkaCertificate = data.KafkaCertificate;
                db.SaveChanges();
                sourceResp.Result = ResultEnum.Success;
            }
            return sourceResp;
        }

        public void Post(PostSourceRequest data)
        {
            using (var db = new DatabaseContext())
            {
                db.Add(new Source
                {
                    //Id = data.Id,
                    //Title = data.Title,
                    Topic = data.Topic,
                    ApiKey = data.ApiKey,
                    Secret = data.Secret,
                    PushServiceReference = data.PushServiceReference,
                    SmsServiceReference = data.SmsServiceReference,
                    EmailServiceReference = data.EmailServiceReference,
                    KafkaUrl=data.KafkaUrl,
                    KafkaCertificate=data.KafkaCertificate,
                    DisplayType=data.DisplayType,
                    Title_EN=data.Title_EN,
                    Title_TR=data.Title_TR,
                    ParentId=data.ParentId,
                    ClientIdJsonPath=data.ClientIdJsonPath
                    
                });

                db.SaveChanges();
            }
        }
    }
}
