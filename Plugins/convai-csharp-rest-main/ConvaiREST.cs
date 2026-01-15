using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Convai.RestAPI.Internal;
using Convai.RestAPI.Internal.PostProcessors;
using Convai.RestAPI.Result;
using Newtonsoft.Json;

namespace Convai.RestAPI
{

#nullable enable
    public static partial class ConvaiREST
    {
        public delegate void HttpClientModifier(HttpClient client);

        private static HttpClientModifier? _httpClientModifier;

        internal static void SetHttpClientModifier(HttpClientModifier modifier) => _httpClientModifier = modifier;

        internal static async Task<(string, bool)> Request(RequestDispatcher.RequestType requestType, Dictionary<string, string> dataToSend, string apiKey, string url, bool useThisURL = false)
        {
            string json = JsonConvert.SerializeObject(dataToSend);
            return await Request(requestType, json, apiKey, url, useThisURL);
        }

        internal static async Task<(string, bool)> Request(RequestDispatcher.RequestType requestType, string dataToSend, string apiKey, string url, bool useThisURL = false)
        {
            if (!RequestPreprocessor.CreateRequest(apiKey, dataToSend, out HttpClient client, out HttpContent content))
            {
                return (string.Empty, false);
            }

            _httpClientModifier?.Invoke(client);
            _httpClientModifier = null;

            string endPoint = useThisURL ? url : ConvaiURL.GetEndPoint(url);
            return await RequestDispatcher.Dispatch(endPoint, requestType, client, content);
        }

        public static async void DownloadFile(string url, Action<byte[]> onSuccess, Action<string>? onError = null)
        {
            using HttpClient client = new HttpClient();
            try
            {
                byte[] bytes = await client.GetByteArrayAsync(url);
                onSuccess.Invoke(bytes);
            }
            catch (Exception e)
            {
                onError?.Invoke(e.Message);
            }
        }

        public class GetCharacterDetailsOperation : OperationResult<CharacterDetails>
        {
            private GetCharacterDetailsModel _model;
            public GetCharacterDetailsOperation(GetCharacterDetailsModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                Dictionary<string, string> dataToSend = new Dictionary<string, string>() { { "charID", _model.CharacterID } };

                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, dataToSend, _model.APIKey, ConvaiURL.CharacterGet);

                if (!result.Item2)
                {
                    SetCompletion(false, CharacterDetails.Default());
                    return;
                }

                if (!RequestPostProcessor.ProcessGetCharacter(result.Item1, out CharacterDetails? details) || details == null)
                {
                    SetCompletion(false, CharacterDetails.Default());
                    return;
                }

                SetCompletion(true, details);
            }
        }
    }
}
