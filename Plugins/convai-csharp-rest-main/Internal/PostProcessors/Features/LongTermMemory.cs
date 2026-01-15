using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Convai.RestAPI.Internal.PostProcessors
{

#nullable enable
    public static partial class RequestPostProcessor
    {
        public static bool ProcessCreateSpeakerID(string result, out string id)
        {
            CreateSpeakerIDResult? speakerIDResult = JsonConvert.DeserializeObject<CreateSpeakerIDResult>(result);
            if (speakerIDResult == null || string.IsNullOrEmpty(speakerIDResult.SpeakerID))
            {
                id = string.Empty;
                return false;
            }

            id = speakerIDResult.SpeakerID;
            return true;
        }

        public static bool ProcessSpeakerIDList(string result, out List<SpeakerIDDetails> speakerIDList)
        {
            speakerIDList = JsonConvert.DeserializeObject<List<SpeakerIDDetails>>(result) ?? throw new InvalidOperationException();
            return true;
        }

        public static bool ProcessLTMStatus(string result, out bool status)
        {
            status = false;
            CharacterDetails? characterGetResponse = JsonConvert.DeserializeObject<CharacterDetails>(result);
            if (characterGetResponse == null)
            {
                return false;
            }

            status = characterGetResponse.MemorySettings.IsEnabled;
            return true;
        }

        public static bool ProcessLTMUpdateStatus(string result, out bool status)
        {
            status = false;
            CharacterUpdateResponse? characterUpdateResponse = JsonConvert.DeserializeObject<CharacterUpdateResponse>(result);
            if (characterUpdateResponse == null)
            {
                return false;
            }

            status = characterUpdateResponse.Status == "SUCCESS";
            return true;
        }
    }

}
