#nullable enable
using Newtonsoft.Json;

namespace Convai.RestAPI.Internal.PostProcessors
{

    public partial class RequestPostProcessor
    {
        public static bool ProcessAnimationList(string result, out ServerAnimationListResponse? animationListResponse)
        {
            animationListResponse = JsonConvert.DeserializeObject<ServerAnimationListResponse>(result);
            return animationListResponse != null;
        }

        public static bool ProcessAnimationData(string result, out ServerAnimationDataResponse? animationDataResponse)
        {
            animationDataResponse = JsonConvert.DeserializeObject<ServerAnimationDataResponse>(result);
            return animationDataResponse != null;
        }
    }

}
