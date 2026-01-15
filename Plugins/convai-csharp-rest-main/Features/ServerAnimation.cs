#nullable enable
using System.Collections.Generic;
using Convai.RestAPI.Internal;
using Convai.RestAPI.Internal.PostProcessors;
using Convai.RestAPI.Result;

namespace Convai.RestAPI
{

    public partial class ConvaiREST
    {
        public class GetAnimationListOperation : OperationResult<ServerAnimationListResponse>
        {
            private GetAnimationListModel _model;
            public GetAnimationListOperation(GetAnimationListModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                Dictionary<string, string> dataToSend = new Dictionary<string, string>()
                {
                    { "status", _model.Status },
                    { "generate_signed_urls", "true" },
                    { "page", _model.Page.ToString() }
                };
                SetHttpClientModifier(client =>
                {
                    client.DefaultRequestHeaders.Add("Source", "convaiUI");
                });
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, dataToSend, _model.APIKey, ConvaiURL.GetAnimationList);

                if (!result.Item2)
                {
                    SetCompletion(false, ServerAnimationListResponse.Default());
                    return;
                }

                if (!RequestPostProcessor.ProcessAnimationList(result.Item1, out ServerAnimationListResponse? animationListResponse) || animationListResponse == null)
                {
                    SetCompletion(false, ServerAnimationListResponse.Default());
                    return;
                }

                SetCompletion(true, animationListResponse);
            }
        }

        public class GetAnimationDataOperation : OperationResult<ServerAnimationDataResponse>
        {
            private GetAnimationItemModel _model;
            public GetAnimationDataOperation(GetAnimationItemModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                Dictionary<string, string> dataToSend = new Dictionary<string, string>()
                {
                    { "animation_id", _model.AnimationID },
                    { "generate_upload_video_urls", "true" }
                };
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, dataToSend, _model.APIKey, ConvaiURL.GetAnimation);

                if (!result.Item2)
                {
                    SetCompletion(false, ServerAnimationDataResponse.Default());
                    return;
                }

                if (!RequestPostProcessor.ProcessAnimationData(result.Item1, out ServerAnimationDataResponse? response) || response == null)
                {
                    SetCompletion(false, ServerAnimationDataResponse.Default());
                    return;
                }

                SetCompletion(true, response);
            }
        }
    }

}
