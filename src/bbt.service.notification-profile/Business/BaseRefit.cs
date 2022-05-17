using Polly;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace bbt.framework.dengage
{
    public abstract class BaseRefit<TInterface>
    {
        public TInterface api;
        public abstract string controllerName { get; }

        ILogger baseLog;

        public BaseRefit(string baseURL, string token, ILogger _baseLog)
        {
            baseLog = _baseLog;
            api = RestClient.For<TInterface>(baseURL, async (request, cancellationToken) =>
            {
                // See if the request has an authorize header
                var auth = request.Headers.Authorization;

                if (auth != null && !string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue(auth.Scheme, token);
                }
            });
        }

        protected async Task<TReturnModel> ExecutePolly<TReturnModel>(Func<TReturnModel> action)
        {

            var policy = Policy
               .Handle<Exception>()
               .WaitAndRetry(new[]
                  {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(5)
                  }, async (exception, timeSpan, retryCount, context) =>
                  {
                      if (exception.Message.Contains("401 (Unauthorized)"))
                      {
                          //ITokenService tokenService = FrameworkDependencyHelper.Instance.Get<ITokenService>();

                          //TokenRefreshRequestModel request = new TokenRefreshRequestModel();
                          //request.token = baseSessionState.Token().Result;
                          //request.refresh_token = baseSessionState.RefreshToken().Result;


                          //TokenResponseModel tokenResponseModel = tokenService.RefreshToken(request).Result;

                          //if (tokenResponseModel.Result == ResultEnum.Success)
                          //{
                          //    await baseSessionState.SetToken(tokenResponseModel);
                          //}
                      }
                      else
                      {
                          baseLog.LogError(exception.Message);
                      }
                  });


            return policy.Execute<TReturnModel>(action);
        }
    }
}
