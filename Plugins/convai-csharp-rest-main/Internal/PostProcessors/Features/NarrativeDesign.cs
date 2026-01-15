using System;
using System.Collections.Generic;
using Convai.RestAPI.Internal.Models;
using Newtonsoft.Json;

namespace Convai.RestAPI.Internal.PostProcessors
{
#nullable enable
    public static partial class RequestPostProcessor
    {
        public static bool ProcessListSections(string result, out List<SectionData>? sectionList)
        {
            sectionList = JsonConvert.DeserializeObject<List<SectionData>>(result);
            return sectionList != null;
        }

        public static bool ProcessListTriggers(string result, out List<TriggerData>? triggerList)
        {
            triggerList = JsonConvert.DeserializeObject<List<TriggerData>>(result);
            return triggerList != null;
        }

        public static bool ProcessCreateSection(string result, out CreateSectionResponse? response)
        {
            response = JsonConvert.DeserializeObject<CreateSectionResponse>(result);
            return response != null && !string.IsNullOrEmpty(response.SectionId);
        }

        public static bool ProcessGetSection(string result, out SectionData? section)
        {
            section = default;

            if (string.IsNullOrWhiteSpace(result) || result.Trim().Equals("{}", StringComparison.Ordinal))
            {
                return true;
            }

            section = JsonConvert.DeserializeObject<SectionData>(result);
            return section != null;
        }

        public static bool ProcessEditSection(string result, out EditSectionResponse? response)
        {
            response = JsonConvert.DeserializeObject<EditSectionResponse>(result);
            return response != null;
        }

        public static bool ProcessStatusResponse(string result, out StatusResponse? response)
        {
            response = JsonConvert.DeserializeObject<StatusResponse>(result);
            return response != null;
        }

        public static bool ProcessCreateTrigger(string result, out TriggerData? trigger)
        {
            trigger = JsonConvert.DeserializeObject<TriggerData>(result);
            return trigger != null;
        }

        public static bool ProcessGetTrigger(string result, out TriggerData? trigger)
        {
            trigger = default;

            if (string.IsNullOrWhiteSpace(result) || result.Trim().Equals("{}", StringComparison.Ordinal))
            {
                return true;
            }

            trigger = JsonConvert.DeserializeObject<TriggerData>(result);
            return trigger != null;
        }
    }
}
