using System.Text;
using Convai.Scripts.LoggerSystem;
using Convai.Scripts.NarrativeDesign;
using Convai.Scripts.RTVI.Outbound;
using Convai.Scripts.Services;
using Convai.Scripts.Services.TranscriptSystem;
using UnityEngine;

namespace Convai.Scripts
{
    public class ConvaiNPC : MonoBehaviour, IConvaiNPCEvents
    {
        [field: SerializeField] public string CharacterName { get; private set; }
        [field: SerializeField] public string CharacterID { get; private set; }
        [field: SerializeField] public ConvaiTranscriptMetaData TranscriptMetaData { get; private set; }

        [Header("Session Resumption")]
        [field: SerializeField]
        public bool EnableSessionResume { get; private set; }

        [Header("Narrative Design")] [SerializeField]
        private ConvaiNarrativeDesignController _narrativeDesignController = new();

        private readonly object _ttsTextLock = new();

        private string _currentMessage = string.Empty;
        private string _llmGeneratedText = string.Empty; // Store LLM text without displaying immediately

        private StringBuilder _progressiveTTSTextBuilder; // avoid per utterance allocation

        public bool IsSpeechMuted =>
            ConvaiRoomManager.Instance != null && ConvaiRoomManager.Instance.IsNpcAudioMuted(this);

        private void OnEnable()
        {
            _progressiveTTSTextBuilder ??= new StringBuilder(256);
            ConvaiServices.CharacterLocatorService.AddNPC(this);
        }

        private void OnDisable() => ConvaiServices.CharacterLocatorService.RemoveNPC(this);

        public void SendTriggerEvent(string triggerName, string triggerMessage = null)
        {
            if (ConvaiRoomManager.Instance.IsConnectedToRoom)
            {
                RTVITriggerMessage trigger = new(triggerName, triggerMessage);
                ConvaiRoomManager.Instance.RTVIHandler.SendData(trigger);
            }
            else
            {
                ConvaiRoomManager.Instance.OnRoomConnectionSuccessful.AddListener(() =>
                {
                    RTVITriggerMessage trigger = new(triggerName, triggerMessage);
                    ConvaiRoomManager.Instance.RTVIHandler.SendData(trigger);
                });
            }
        }

        public void ToggleSpeech()
        {
            if (IsSpeechMuted)
            {
                UnmuteSpeech();
            }
            else
            {
                MuteSpeech();
            }
        }

        public bool MuteSpeech()
        {
            if (ConvaiRoomManager.Instance == null)
            {
                ConvaiUnityLogger.DebugLog($"[{CharacterName}] [{CharacterID}] Cannot mute speech - room manager not available.",
                    LogCategory.SDK);
                return false;
            }

            return ConvaiRoomManager.Instance.MuteNpc(this);
        }

        public bool UnmuteSpeech()
        {
            if (ConvaiRoomManager.Instance == null)
            {
                ConvaiUnityLogger.DebugLog($"[{CharacterName}] [{CharacterID}] Cannot unmute speech - room manager not available.",
                    LogCategory.SDK);
                return false;
            }

            return ConvaiRoomManager.Instance.UnmuteNpc(this);
        }

        #region IConvaiNPCEvents Implementation

        public void OnCharacterTranscriptionReceived(string transcript, bool isFinal)
        {
            if (string.IsNullOrEmpty(transcript))
            {
                ConvaiUnityLogger.DebugLog($"[{CharacterName}] [{CharacterID}] received empty transcript", LogCategory.Character);
                return;
            }

            // Store LLM-generated text but don't display it immediately
            // The progressive TTS text will handle the actual display synchronously with speech
            if (isFinal)
            {
                _llmGeneratedText = _currentMessage + transcript;
                _currentMessage = _llmGeneratedText;
                ConvaiUnityLogger.DebugLog($"[{CharacterName}] [{CharacterID}] LLM final transcript stored (not displayed): '{_llmGeneratedText}'",
                    LogCategory.Character);
            }
            else
            {
                // Store interim LLM text but don't broadcast to prevent premature display
                ConvaiUnityLogger.DebugLog(
                    $"[{CharacterName}] [{CharacterID}] LLM interim transcript stored (not displayed): '{_currentMessage + transcript}'",
                    LogCategory.Character);
            }
        }

        public void OnCharacterStartedSpeaking() =>
            ConvaiUnityLogger.DebugLog($"[{CharacterName}] [{CharacterID}] Started speaking.", LogCategory.SDK);

        public void OnCharacterStoppedSpeaking()
        {
            ConvaiUnityLogger.DebugLog($"[{CharacterName}] [{CharacterID}] Stopped speaking.", LogCategory.SDK);
            _progressiveTTSTextBuilder?.Clear();
            _currentMessage = string.Empty;
            ConvaiServices.TranscriptService.BroadcastCharacterMessage(CharacterID, CharacterName, _currentMessage, true);
        }

        public void OnLLMStarted()
        {
            ConvaiUnityLogger.DebugLog($"[{CharacterName}] [{CharacterID}] LLM Started.", LogCategory.SDK);
            _currentMessage = string.Empty; // Reset current message when LLM starts
        }

        public void OnLLMStopped() => ConvaiUnityLogger.DebugLog($"[{CharacterName}] [{CharacterID}] LLM Stopped.", LogCategory.SDK);

        public void OnTTSStarted()
        {
            ConvaiUnityLogger.DebugLog($"[{CharacterName}] [{CharacterID}] TTS Started.", LogCategory.SDK);
            lock (_ttsTextLock)
            {
                _progressiveTTSTextBuilder ??= new StringBuilder(256);
                _progressiveTTSTextBuilder.Clear(); // Reset progressive text buffer when TTS starts
            }
        }

        public void OnTTSStopped() => ConvaiUnityLogger.DebugLog($"[{CharacterName}] [{CharacterID}] TTS Stopped.", LogCategory.SDK);

        public void OnTTSTextReceived(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return;
            }

            lock (_ttsTextLock)
            {
                _progressiveTTSTextBuilder = new StringBuilder(256);

                // Append word with space separation to build progressive transcript
                if (_progressiveTTSTextBuilder.Length > 0)
                {
                    _progressiveTTSTextBuilder.Append(' ');
                }

                _progressiveTTSTextBuilder.Append(word);

                // Broadcast the progressive transcript to Unity's main event system
                string progressiveTranscript = _progressiveTTSTextBuilder.ToString();
                ConvaiServices.TranscriptService.BroadcastCharacterMessage(CharacterID, CharacterName, progressiveTranscript, false);

                ConvaiUnityLogger.DebugLog($"[{CharacterName}] [{CharacterID}] TTS Text received: '{word}' - Progressive: '{progressiveTranscript}'",
                    LogCategory.SDK);
            }
        }

        public void OnCurrentNarrativeDesignSectionIDReceived(string sectionID) =>
            _narrativeDesignController.OnNarrativeDesignSectionReceived(sectionID);

        #endregion
    }
}
