using System;

namespace Convai.Scripts.Services.TranscriptSystem
{
    public class ConvaiTranscriptService
    {
        private event Action<ConvaiTranscriptData> OnCharacterMessage = data => { };
        private event Action<ConvaiTranscriptData> OnPlayerMessage = data => { };
        private event Action<string, string> OnInteractionIDCreated = (s1, s2) => { };

        public void BroadcastCharacterMessage(string charID, string charName, string message, bool isLastMessage) =>
            OnCharacterMessage(new ConvaiTranscriptData(charID, charName, message, isLastMessage));

        public void BroadcastPlayerMessage(string speakerID, string playerName, string transcript, bool finalTranscript) =>
            OnPlayerMessage(new ConvaiTranscriptData(speakerID, playerName, transcript, finalTranscript));

        public void BroadcastInteractionIDCreated(string characterId, string interactionID) => OnInteractionIDCreated(characterId, interactionID);

        public void SetCharacterMessageSubscriptionState(Action<ConvaiTranscriptData> callback, bool newState)
        {
            if (newState)
            {
                OnCharacterMessage += callback;
            }
            else
            {
                OnCharacterMessage -= callback;
            }
        }

        public void SetPlayerMessageSubscriptionState(Action<ConvaiTranscriptData> callback, bool newState)
        {
            if (newState)
            {
                OnPlayerMessage += callback;
            }
            else
            {
                OnPlayerMessage -= callback;
            }
        }

        public void SetInteractionIDCreatedState(Action<string, string> callback, bool newState)
        {
            if (newState)
            {
                OnInteractionIDCreated += callback;
            }
            else
            {
                OnInteractionIDCreated -= callback;
            }
        }
    }
}
