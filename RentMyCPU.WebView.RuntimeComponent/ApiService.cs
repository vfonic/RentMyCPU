using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Security.ExchangeActiveSyncProvisioning;
using System.Net;
using Windows.Web.Http.Filters;
using Windows.Security.Cryptography.Certificates;

namespace RentMyCPU.WebView.RuntimeComponent
{
    public sealed class ApiService
    {
        private static ApiService _instance;
        public static ApiService Instance => _instance ?? (_instance = new ApiService());
#if DEBUG
        public string ApiAddress => "https://localhost:5001/";
#else
        public string ApiAddress => "https://rentmycpu.azurewebsites.net/";
#endif  
        public IAsyncOperation<HttpResponseMessage> AddCredits(Guid transactionId, string receipt, string token)
        {
            return AsyncInfo.Run(async cancellationToken =>
            {
                try
                {
                    var filter = new HttpBaseProtocolFilter();
#if DEBUG
                    filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);
                    filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
                    filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidName);
#endif
                    var client = new HttpClient(filter);
                    var message = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(ApiAddress + "api/credits/" + transactionId.ToString()),
                        Content = new HttpStringContent(receipt, Windows.Storage.Streams.UnicodeEncoding.Utf8, "text/xml")
                    };
                    message.Headers.Authorization = new HttpCredentialsHeaderValue("Bearer", token);
                    var result = await client.SendRequestAsync(message);
                    return result;
                }
                catch (Exception e)
                {
                    throw;
                }
            });
        }

        public IAsyncOperation<HttpResponseMessage> IsPurchaseFulfilled(Guid transactionId, string token)
        {
            return AsyncInfo.Run(async cancellationToken =>
            {
                try
                {
                    var filter = new HttpBaseProtocolFilter();
#if DEBUG
                    filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);
                    filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
                    filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidName);
#endif
                    var client = new HttpClient(filter);
                    var message = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri(ApiAddress + "api/credits/fulfilled/" + transactionId.ToString())
                    };
                    message.Headers.Authorization = new HttpCredentialsHeaderValue("Bearer", token);
                    var result = await client.SendRequestAsync(message);
                    return result;
                }
                catch (Exception e)
                {
                    throw;
                }
            });
        }
    }
}
