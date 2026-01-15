using System.Collections.Generic;
using Convai.Scripts.LoggerSystem;
using Convai.Scripts.Player;
using Convai.Scripts.Services;
using Convai.Scripts.Services.TranscriptSystem;
using Convai.Scripts.TranscriptUI.Filters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Convai.Scripts.TranscriptUI.Chat_UI
{
    public class ConvaiChatUI : ConvaiTranscriptUIBase
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform chatContainer;
        [SerializeField] private ConvaiMessageUI characterChatMessageUI;
        [SerializeField] private ConvaiMessageUI playerChatMessageUI;
        [SerializeField] private TMP_InputField chatInputField;

        private Dictionary<string, ConvaiMessageUI> _activeMessages = new();
        private ConvaiProximityNPCFilter _filter;
        private ConvaiMessageUI _lastCharacterChatMessageUI;

        public override void OnActivate()
        {
            base.OnActivate();
            // Ensure filter is set up
            if (!TranscriptHandler.gameObject.TryGetComponent(out _filter))
            {
                _filter = TranscriptHandler.gameObject.AddComponent<ConvaiProximityNPCFilter>();
            }

            _activeMessages = new Dictionary<string, ConvaiMessageUI>();
            chatInputField.onSubmit.AddListener(ChatInputField_OnSubmit);
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
            _activeMessages.Clear();
            if (_filter != null)
            {
                Destroy(_filter);
                _filter = null;
            }
        }

        private void ChatInputField_OnSubmit(string text)
        {
            if (TranscriptHandler.ConvaiPlayer == null)
            {
                ConvaiUnityLogger.Info("No convai player found", LogCategory.UI);
                return;
            }

            chatInputField.SetTextWithoutNotify(string.Empty);
            //TranscriptHandler.ConvaiPlayer.SendTextMessage(text);
        }


        protected override void OnVisibleCharacterIDChanged(string id, bool newState)
        {
            if (newState)
            {
                if (_lastCharacterChatMessageUI != null && _lastCharacterChatMessageUI.Identifier == id && !_lastCharacterChatMessageUI.IsCompleted)
                {
                    _activeMessages.Add(id, _lastCharacterChatMessageUI);
                }
            }
            else
            {
                RemoveActiveMessage(id);
            }

            HandleFading();
        }

        protected override void OnCharacterMessage(ConvaiTranscriptData transcript)
        {
            if (!TranscriptHandler.visibleCharacterChatIds.Contains(transcript.Identifier))
            {
                return;
            }

            if (_activeMessages.TryGetValue(transcript.Identifier, out ConvaiMessageUI ui))
            {
                ConvaiUnityLogger.DebugLog($"Updating existing message for {transcript.Identifier}", LogCategory.UI);
                if (!string.IsNullOrEmpty(transcript.Message))
                {
                    ConvaiUnityLogger.DebugLog($"Message for {transcript.Identifier} is not empty, updating UI.", LogCategory.UI);
                    ui.SetMessage(transcript.Message);
                }

                RemoveActiveMessage(ref transcript);
                ScrollToBottom();
            }
            else
            {
                if (string.IsNullOrEmpty(transcript.Message))
                {
                    return;
                }

                ConvaiUnityLogger.DebugLog($"Creating new message for {transcript.Identifier}", LogCategory.UI);
                ConvaiMessageUI newChatMessage = CreateNewMessage(characterChatMessageUI, transcript.Identifier);
                _lastCharacterChatMessageUI = newChatMessage;
                if (ConvaiServices.CharacterLocatorService.GetNPC(transcript.Identifier, out ConvaiNPC npc))
                {
                    InitializeMessageUI(newChatMessage, ref transcript, npc.TranscriptMetaData);
                }
                else
                {
                    InitializeMessageUI(newChatMessage, ref transcript, new ConvaiTranscriptMetaData());
                }

                ScrollToBottom();
            }
        }


        protected override void OnPlayerMessage(ConvaiTranscriptData transcript)
        {
            if (_activeMessages.TryGetValue(transcript.Identifier, out ConvaiMessageUI ui))
            {
                ui.SetMessage(transcript.Message);
                RemoveActiveMessage(ref transcript);
                ScrollToBottom();
            }
            else
            {
                ConvaiMessageUI newChatMessage = CreateNewMessage(playerChatMessageUI, transcript.Identifier);
                if (ConvaiServices.CharacterLocatorService.GetPlayer(transcript.Identifier, out ConvaiPlayer player))
                {
                    InitializeMessageUI(newChatMessage, ref transcript, player.TranscriptMetaData);
                }
                else
                {
                    InitializeMessageUI(newChatMessage, ref transcript, new ConvaiTranscriptMetaData());
                }

                ScrollToBottom();
            }
        }

        private void RemoveActiveMessage(ref ConvaiTranscriptData transcript)
        {
            if (!transcript.IsLastChunk)
            {
                return;
            }

            ConvaiUnityLogger.DebugLog($"Removing active message for {transcript.Identifier}", LogCategory.UI);
            _activeMessages[transcript.Identifier].IsCompleted = true;
            RemoveActiveMessage(transcript.Identifier);
        }

        private void RemoveActiveMessage(string identifier) => _activeMessages.Remove(identifier);


        private ConvaiMessageUI CreateNewMessage(ConvaiMessageUI prefab, string identifier)
        {
            ConvaiMessageUI newChatMessage = Instantiate(prefab, chatContainer.transform);
            newChatMessage.gameObject.SetActive(true);
            _activeMessages.Add(identifier, newChatMessage);
            return newChatMessage;
        }

        private void InitializeMessageUI(ConvaiMessageUI newChatMessage, ref ConvaiTranscriptData transcript,
            ConvaiTranscriptMetaData transcriptMetaData)
        {
            newChatMessage.Identifier = transcript.Identifier;
            newChatMessage.SetSender(transcript.Name);
            newChatMessage.SetMessage(transcript.Message);
            newChatMessage.SetSenderColor(transcriptMetaData.nameTagColor);
        }

        private void ScrollToBottom() => scrollRect.verticalNormalizedPosition = 0;

        protected override void OnInteractionIDCreated(string characterId, string interactionID)
        {
            if (_activeMessages.TryGetValue(characterId, out ConvaiMessageUI ui))
            {
                ui.SetInteractionID(interactionID);
            }
        }
    }
}
