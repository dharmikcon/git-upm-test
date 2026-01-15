using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Convai.RestAPI.Internal
{

    internal static class RequestDispatcher
    {
        internal enum RequestType
        {
            Post,
            Get
        }
#nullable enable
        internal static async Task<(string, bool)> Dispatch(string url, RequestType requestType, HttpClient httpClient, HttpContent? httpContent = null)
        {
            try
            {
                HttpResponseMessage? response = requestType switch
                {
                    RequestType.Post => await httpClient.PostAsync(url, httpContent),
                    RequestType.Get => await httpClient.GetAsync(url),
                    _ => throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null)
                };
                if (response == null)
                {
                    return (string.Empty, false);
                }

                response.EnsureSuccessStatusCode();
                return (await response.Content.ReadAsStringAsync(), true);
            }
            catch (Exception e)
            {
                return (e.Message + Environment.NewLine + e.StackTrace, false);
            }
        }
    }

}
