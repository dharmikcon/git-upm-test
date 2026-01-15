using Newtonsoft.Json;

namespace Convai.RestAPI.Internal.PostProcessors
{

#nullable enable
    public static partial class RequestPostProcessor
    {
        public static bool ProcessGetCharacter(string result, out CharacterDetails? characterDetails)
        {
            characterDetails = JsonConvert.DeserializeObject<CharacterDetails>(result);
            return characterDetails != null;
        }
    }

}
