using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Convai.RestAPI
{

    public class UserUsageData
    {
        public UserUsageData(UsageData usage) => Usage = usage;

        public static UserUsageData Default()
        {
            return new UserUsageData(UsageData.Default());
        }

        [JsonProperty("usage")] public UsageData Usage { get; set; }

        public class ProviderPool
        {
            public ProviderPool(List<string> providers, UsageLimits usageLimits, UsageData usage)
            {
                Providers = providers;
                UsageLimits = usageLimits;
                Usage = usage;
            }

            public static ProviderPool Default()
            {
                return new ProviderPool(new List<string>(), new UsageLimits(0), UsageData.Default());
            }

            [JsonProperty("providers")] public List<string> Providers { get; set; }
            [JsonProperty("usage_limits")] public UsageLimits UsageLimits { get; set; }
            [JsonProperty("usage")] public UsageData Usage { get; set; }
        }

        public class TextToSpeech
        {
            public TextToSpeech(string standaloneTts, ProviderPool providerPool)
            {
                StandaloneTTS = standaloneTts;
                ProviderPool = providerPool;
            }

            public static TextToSpeech Default()
            {
                return new TextToSpeech(string.Empty, ProviderPool.Default());
            }

            [JsonProperty("standalone_tts")] public string StandaloneTTS { get; set; }
            [JsonProperty("provider_pool_1")] public ProviderPool ProviderPool { get; set; }
        }

        public class UsageData
        {
            public UsageData(string planName, DateTime expiryTs, int dailyLimit, int monthlyLimit, bool extendedIsAllowed, TextToSpeech textToSpeech, int dailyUsage, int monthlyUsage, int extendedUsage, int monthly)
            {
                PlanName = planName;
                ExpiryTs = expiryTs;
                DailyLimit = dailyLimit;
                MonthlyLimit = monthlyLimit;
                ExtendedIsAllowed = extendedIsAllowed;
                TextToSpeech = textToSpeech;
                DailyUsage = dailyUsage;
                MonthlyUsage = monthlyUsage;
                ExtendedUsage = extendedUsage;
                Monthly = monthly;
            }

            public static UsageData Default()
            {
                return new UsageData(string.Empty, DateTime.MinValue, 0, 0, false, TextToSpeech.Default(), 0, 0, 0, 0);
            }

            [JsonProperty("plan_name")] public string PlanName { get; set; }
            [JsonProperty("expiry_ts")] public DateTime ExpiryTs { get; set; }
            [JsonProperty("daily_limit")] public int DailyLimit { get; set; }
            [JsonProperty("monthly_limit")] public int MonthlyLimit { get; set; }
            [JsonProperty("extended_isAllowed")] public bool ExtendedIsAllowed { get; set; }
            [JsonProperty("tts")] public TextToSpeech TextToSpeech { get; set; }
            [JsonProperty("daily_usage")] public int DailyUsage { get; set; }
            [JsonProperty("monthly_usage")] public int MonthlyUsage { get; set; }
            [JsonProperty("extended_usage")] public int ExtendedUsage { get; set; }
            [JsonProperty("monthly")] public int Monthly { get; set; }
        }

        public class UsageLimits
        {
            public UsageLimits(int monthly) => Monthly = monthly;

            public static UsageLimits Default()
            {
                return new UsageLimits(0);
            }
            [JsonProperty("monthly")] public int Monthly { get; set; }
        }
    }

}
