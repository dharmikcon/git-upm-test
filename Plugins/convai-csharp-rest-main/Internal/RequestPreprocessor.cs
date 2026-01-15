using System.Text;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Convai.RestAPI.Internal
{

    internal static class RequestPreprocessor
    {
        private static bool CreateClient(string apiKey, out HttpClient httpClient)
        {
            httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30), DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } } };
            if (string.IsNullOrEmpty(apiKey))
            {
                return false;
            }

            httpClient.DefaultRequestHeaders.Add("CONVAI-API-KEY", apiKey);
            return true;
        }

        private static HttpContent CreateContent(string json) => new StringContent(json, Encoding.UTF8, "application/json");


        internal static bool CreateRequest(string apiKey, string json, out HttpClient httpClient, out HttpContent httpContent)
        {
            httpContent = CreateContent(json);
            return CreateClient(apiKey, out httpClient);
        }
    }

}
