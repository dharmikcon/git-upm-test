using Newtonsoft.Json;

namespace Convai.RestAPI.Internal.PostProcessors
{

#nullable enable
    public static partial class RequestPostProcessor
    {
        public static bool ProcessAPIValidation(string result, out ReferralSourceStatus? referralSourceStatus)
        {
            referralSourceStatus = JsonConvert.DeserializeObject<ReferralSourceStatus>(result);
            return referralSourceStatus != null;
        }

        public static bool ProcessUserAPIUsage(string result, out UserUsageData? userUsageData)
        {
            userUsageData = JsonConvert.DeserializeObject<UserUsageData>(result);
            return userUsageData != null;
        }
    }

}
