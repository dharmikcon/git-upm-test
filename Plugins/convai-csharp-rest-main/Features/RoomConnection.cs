using System;
using System.Threading.Tasks;
using Convai.RestAPI.Internal;
using Convai.RestAPI.Internal.PostProcessors;
using Convai.RestAPI.Result;
using Newtonsoft.Json;

namespace Convai.RestAPI
{
    public static partial class ConvaiREST
    {
        public class GetRoomConnectionOperation : OperationResult<RoomDetails>
        {
            private ConvaiRoomRequest _model;
            public GetRoomConnectionOperation(ConvaiRoomRequest model)
            {
                _model = model;
                _ = Execute();
            }
            private async Task Execute()
            {
                try
                {
                    string dataToSend = JsonConvert.SerializeObject(_model);
                    SetHttpClientModifier(client =>
                    {
                        client.DefaultRequestHeaders.Add("x-api-key", _model.APIKey);
                    });
                    (string, bool) result = await Request(RequestDispatcher.RequestType.Post, dataToSend, _model.APIKey, _model.URL, true);

                    if (!result.Item2)
                    {
                        SetCompletion(false, RoomDetails.Default(), result.Item1);
                        return;
                    }

                    if (!RequestPostProcessor.ProcessRoomDetails(result.Item1, out RoomDetails? details) || details == null)
                    {
                        SetCompletion(false, RoomDetails.Default(), result.Item1);
                        return;
                    }

                    SetCompletion(true, details);
                }
                catch (Exception ex)
                {
                    SetCompletion(false, RoomDetails.Default(), ex.Message);
                }
            }
        }
    }
}
