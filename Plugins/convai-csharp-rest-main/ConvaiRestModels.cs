using System;
using System.Collections.Generic;
using Convai.RestAPI.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Convai.RestAPI
{

#nullable enable

    [Serializable]
    public class ConvaiModel
    {
        protected ConvaiModel()
        {
        }

        public ConvaiModel(string apiKey) => APIKey = apiKey;

        [JsonIgnore]
        public string APIKey { get; protected set; } = string.Empty;
    }

    [Serializable]
    public class CreateSpeakerIDModel : ConvaiModel
    {
        public CreateSpeakerIDModel(string apiKey, string playerName) : base(apiKey)
        {
            PlayerName = playerName;
        }

        public CreateSpeakerIDModel(string apiKey, string playerName, string? deviceId) : base(apiKey)
        {
            PlayerName = playerName;
            DeviceId = deviceId;
        }

        public string PlayerName { get; private set; }

        public string? DeviceId { get; private set; }
    }

    [Serializable]
    public class DeleteSpeakerIDModel : ConvaiModel
    {
        public DeleteSpeakerIDModel(string apiKey, string speakerID) : base(apiKey) => SpeakerID = speakerID;

        public string SpeakerID { get; private set; }
    }

    [Serializable]
    public class GetCharacterDetailsModel : ConvaiModel
    {
        public GetCharacterDetailsModel(string apiKey, string characterID) : base(apiKey) => CharacterID = characterID;

        public string CharacterID { get; private set; }
    }

    [Serializable]
    public class UpdateLTMStatusModel : ConvaiModel
    {
        public UpdateLTMStatusModel(string apiKey, string characterID, bool status) : base(apiKey)
        {
            CharacterID = characterID;
            Status = status;
        }

        public string CharacterID { get; private set; }
        public bool Status { get; private set; }
    }

    [Serializable]
    public class UpdateReferralSourceModel : ConvaiModel
    {
        public UpdateReferralSourceModel(string apiKey, string source) : base(apiKey) => Source = source;

        public string Source { get; private set; }
    }

    [Serializable]
    public class NarrativeDesignListModel : ConvaiModel
    {
        public NarrativeDesignListModel(string apiKey, string characterID) : base(apiKey) => CharacterID = characterID;

        public string CharacterID { get; set; }
    }

    [Serializable]
    internal class CharacterUpdateRequest
    {
        public CharacterUpdateRequest(string characterID, bool isEnabled)
        {
            CharacterID = characterID;
            MemorySettings = new MemorySettings(isEnabled);
        }

        [JsonProperty("charID")] public string CharacterID { get; set; }
        [JsonProperty("memorySettings")] public MemorySettings MemorySettings { get; set; }
    }

    [Serializable]
    public class GetAnimationListModel : ConvaiModel
    {
        public GetAnimationListModel(string apiKey, int page, string status) : base(apiKey)
        {
            Page = page;
            Status = status;
        }

        public int Page { get; private set; }

        public string Status { get; private set; }
    }

    [Serializable]
    public class GetAnimationItemModel : ConvaiModel
    {
        public GetAnimationItemModel(string apiKey, string animationID) : base(apiKey) => AnimationID = animationID;
        public string AnimationID { get; private set; }
    }


    [Serializable]
    public class ConvaiRoomRequest : ConvaiModel
    {
        public ConvaiRoomRequest(
            string apiKey,
            string characterID,
            string transport,
            string connectionType,
            string llmProvider,
            string url,
            string? characterSessionId = null,
            string? endUserId = null)
            : base(apiKey)
        {
            CharacterID = characterID;
            Transport = transport;
            ConnectionType = connectionType;
            LLMProvider = llmProvider;
            URL = url;
            CharacterSessionId = characterSessionId;
            EndUserId = endUserId;
        }

        [JsonProperty("character_id")]
        public string CharacterID { get; set; }

        [JsonProperty("transport")]
        public string Transport { get; set; }

        [JsonProperty("connection_type")]
        public string ConnectionType { get; set; }

        [JsonProperty("llm_provider")]
        public string LLMProvider { get; set; }

        [JsonProperty("core_service_url")]
        public string URL;

        [JsonProperty("character_session_id", NullValueHandling = NullValueHandling.Ignore)]
        public string? CharacterSessionId { get; set; }

        [JsonProperty("end_user_id", NullValueHandling = NullValueHandling.Ignore)]
        public string? EndUserId { get; set; }

        // Multiplayer support properties
        [JsonProperty("max_num_participants", NullValueHandling = NullValueHandling.Ignore)]
        public string? MaxNumParticipants { get; set; }

        [JsonProperty("room_name", NullValueHandling = NullValueHandling.Ignore)]
        public string? RoomName { get; set; }

        [JsonProperty("mode", NullValueHandling = NullValueHandling.Ignore)]
        public string? Mode { get; set; }

        [JsonProperty("spawn_agent", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SpawnAgent { get; set; }
    }

    [Serializable]
    public class ToggleNarrativeDrivenModel : ConvaiModel
    {
        public ToggleNarrativeDrivenModel(string apiKey, string characterID, bool isNarrativeDriven) : base(apiKey)
        {
            CharacterID = characterID;
            IsNarrativeDriven = isNarrativeDriven;
        }

        [JsonProperty("character_id")]
        public string CharacterID { get; private set; }

        [JsonProperty("is_narrative_driven")]
        public bool IsNarrativeDriven { get; private set; }
    }

    [Serializable]
    public class CreateNarrativeSectionModel : ConvaiModel
    {
        public CreateNarrativeSectionModel(
            string apiKey,
            string characterID,
            string sectionName,
            string objective,
            JObject? updatedCharacterData = null,
            string? behaviorTreeCode = null,
            string? btConstants = null,
            IList<float>? nodePosition = null) : base(apiKey)
        {
            CharacterID = characterID;
            SectionName = sectionName;
            Objective = objective;
            UpdatedCharacterData = updatedCharacterData;
            BehaviorTreeCode = behaviorTreeCode;
            BTConstants = btConstants;
            NodePosition = nodePosition;
        }

        [JsonProperty("character_id")]
        public string CharacterID { get; private set; }

        [JsonProperty("section_name")]
        public string SectionName { get; private set; }

        [JsonProperty("objective")]
        public string Objective { get; private set; }

        [JsonProperty("updated_character_data", NullValueHandling = NullValueHandling.Ignore)]
        public JObject? UpdatedCharacterData { get; private set; }

        [JsonProperty("behavior_tree_code", NullValueHandling = NullValueHandling.Ignore)]
        public string? BehaviorTreeCode { get; private set; }

        [JsonProperty("bt_constants", NullValueHandling = NullValueHandling.Ignore)]
        public string? BTConstants { get; private set; }

        [JsonProperty("node_position", NullValueHandling = NullValueHandling.Ignore)]
        public IList<float>? NodePosition { get; private set; }
    }

    [Serializable]
    public class NarrativeDecisionData
    {
        public NarrativeDecisionData(string criteria, string nextSectionId, int? priority = null)
        {
            Criteria = criteria;
            NextSectionId = nextSectionId;
            Priority = priority;
        }

        [JsonProperty("criteria")]
        public string Criteria { get; set; }

        [JsonProperty("next_section_id")]
        public string NextSectionId { get; set; }

        [JsonProperty("priority", NullValueHandling = NullValueHandling.Ignore)]
        public int? Priority { get; set; }
    }

    [Serializable]
    public class NarrativeSectionUpdateData
    {
        public NarrativeSectionUpdateData(string? sectionName = null, string? objective = null, List<NarrativeDecisionData>? decisions = null)
        {
            SectionName = sectionName;
            Objective = objective;
            Decisions = decisions;
        }

        [JsonProperty("section_name", NullValueHandling = NullValueHandling.Ignore)]
        public string? SectionName { get; set; }

        [JsonProperty("objective", NullValueHandling = NullValueHandling.Ignore)]
        public string? Objective { get; set; }

        [JsonProperty("decisions", NullValueHandling = NullValueHandling.Ignore)]
        public List<NarrativeDecisionData>? Decisions { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken?> AdditionalData { get; set; } = new Dictionary<string, JToken?>();
    }

    [Serializable]
    public class EditNarrativeSectionModel : ConvaiModel
    {
        public EditNarrativeSectionModel(string apiKey, string characterID, string sectionID, NarrativeSectionUpdateData updatedData) : base(apiKey)
        {
            CharacterID = characterID;
            SectionID = sectionID;
            UpdatedData = updatedData;
        }

        [JsonProperty("character_id")]
        public string CharacterID { get; private set; }

        [JsonProperty("section_id")]
        public string SectionID { get; private set; }

        [JsonProperty("updated_data")]
        public NarrativeSectionUpdateData UpdatedData { get; private set; }
    }

    [Serializable]
    public class GetNarrativeSectionModel : ConvaiModel
    {
        public GetNarrativeSectionModel(string apiKey, string characterID, string sectionID) : base(apiKey)
        {
            CharacterID = characterID;
            SectionID = sectionID;
        }

        [JsonProperty("character_id")]
        public string CharacterID { get; private set; }

        [JsonProperty("section_id")]
        public string SectionID { get; private set; }
    }

    [Serializable]
    public class DeleteNarrativeSectionModel : ConvaiModel
    {
        public DeleteNarrativeSectionModel(string apiKey, string characterID, string sectionID) : base(apiKey)
        {
            CharacterID = characterID;
            SectionID = sectionID;
        }

        [JsonProperty("character_id")]
        public string CharacterID { get; private set; }

        [JsonProperty("section_id")]
        public string SectionID { get; private set; }
    }

    [Serializable]
    public class CreateNarrativeTriggerModel : ConvaiModel
    {
        public CreateNarrativeTriggerModel(string apiKey, string characterID, string triggerName, string triggerMessage, string? destinationSection = null, IList<float>? nodePosition = null) : base(apiKey)
        {
            CharacterID = characterID;
            TriggerName = triggerName;
            TriggerMessage = triggerMessage;
            DestinationSection = destinationSection;
            NodePosition = nodePosition;
        }

        [JsonProperty("character_id")]
        public string CharacterID { get; private set; }

        [JsonProperty("trigger_name")]
        public string TriggerName { get; private set; }

        [JsonProperty("trigger_message")]
        public string TriggerMessage { get; private set; }

        [JsonProperty("destination_section", NullValueHandling = NullValueHandling.Ignore)]
        public string? DestinationSection { get; private set; }

        [JsonProperty("node_position", NullValueHandling = NullValueHandling.Ignore)]
        public IList<float>? NodePosition { get; private set; }
    }

    [Serializable]
    public class NarrativeTriggerUpdateData
    {
        public NarrativeTriggerUpdateData(string? triggerName = null, string? triggerMessage = null, string? destinationSection = null)
        {
            TriggerName = triggerName;
            TriggerMessage = triggerMessage;
            DestinationSection = destinationSection;
        }

        [JsonProperty("trigger_name", NullValueHandling = NullValueHandling.Ignore)]
        public string? TriggerName { get; set; }

        [JsonProperty("trigger_message", NullValueHandling = NullValueHandling.Ignore)]
        public string? TriggerMessage { get; set; }

        [JsonProperty("destination_section", NullValueHandling = NullValueHandling.Ignore)]
        public string? DestinationSection { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken?> AdditionalData { get; set; } = new Dictionary<string, JToken?>();
    }

    [Serializable]
    public class UpdateNarrativeTriggerModel : ConvaiModel
    {
        public UpdateNarrativeTriggerModel(string apiKey, string characterID, string triggerID, NarrativeTriggerUpdateData updatedData) : base(apiKey)
        {
            CharacterID = characterID;
            TriggerID = triggerID;
            UpdatedData = updatedData;
        }

        [JsonProperty("character_id")]
        public string CharacterID { get; private set; }

        [JsonProperty("trigger_id")]
        public string TriggerID { get; private set; }

        [JsonProperty("updated_data")]
        public NarrativeTriggerUpdateData UpdatedData { get; private set; }
    }

    [Serializable]
    public class GetNarrativeTriggerModel : ConvaiModel
    {
        public GetNarrativeTriggerModel(string apiKey, string characterID, string triggerID) : base(apiKey)
        {
            CharacterID = characterID;
            TriggerID = triggerID;
        }

        [JsonProperty("character_id")]
        public string CharacterID { get; private set; }

        [JsonProperty("trigger_id")]
        public string TriggerID { get; private set; }
    }

    [Serializable]
    public class DeleteNarrativeTriggerModel : ConvaiModel
    {
        public DeleteNarrativeTriggerModel(string apiKey, string characterID, string triggerID) : base(apiKey)
        {
            CharacterID = characterID;
            TriggerID = triggerID;
        }

        [JsonProperty("character_id")]
        public string CharacterID { get; private set; }

        [JsonProperty("trigger_id")]
        public string TriggerID { get; private set; }
    }

    [Serializable]
    public class AddNarrativeDecisionModel : ConvaiModel
    {
        public AddNarrativeDecisionModel(string apiKey, string characterID, string fromSectionId, string toSectionId, string criteria, int? priority = null) : base(apiKey)
        {
            CharacterID = characterID;
            FromSectionId = fromSectionId;
            ToSectionId = toSectionId;
            Criteria = criteria;
            Priority = priority;
        }

        [JsonProperty("character_id")]
        public string CharacterID { get; private set; }

        [JsonProperty("from_section_id")]
        public string FromSectionId { get; private set; }

        [JsonProperty("to_section_id")]
        public string ToSectionId { get; private set; }

        [JsonProperty("criteria")]
        public string Criteria { get; private set; }

        [JsonProperty("priority", NullValueHandling = NullValueHandling.Ignore)]
        public int? Priority { get; private set; }
    }

    [Serializable]
    public class NarrativeDecisionUpdatePayload
    {
        public NarrativeDecisionUpdatePayload(string? criteria = null, string? nextSectionId = null, int? priority = null)
        {
            Criteria = criteria;
            NextSectionId = nextSectionId;
            Priority = priority;
        }

        [JsonProperty("criteria", NullValueHandling = NullValueHandling.Ignore)]
        public string? Criteria { get; set; }

        [JsonProperty("next_section_id", NullValueHandling = NullValueHandling.Ignore)]
        public string? NextSectionId { get; set; }

        [JsonProperty("priority", NullValueHandling = NullValueHandling.Ignore)]
        public int? Priority { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken?> AdditionalData { get; set; } = new Dictionary<string, JToken?>();
    }

    [Serializable]
    public class EditNarrativeDecisionModel : ConvaiModel
    {
        public EditNarrativeDecisionModel(string apiKey, string characterID, string fromSectionId, string toSectionId, string criteria, NarrativeDecisionUpdatePayload updatedData) : base(apiKey)
        {
            CharacterID = characterID;
            FromSectionId = fromSectionId;
            ToSectionId = toSectionId;
            Criteria = criteria;
            UpdatedData = updatedData;
        }

        [JsonProperty("character_id")]
        public string CharacterID { get; private set; }

        [JsonProperty("from_section_id")]
        public string FromSectionId { get; private set; }

        [JsonProperty("to_section_id")]
        public string ToSectionId { get; private set; }

        [JsonProperty("criteria")]
        public string Criteria { get; private set; }

        [JsonProperty("updated_data")]
        public NarrativeDecisionUpdatePayload UpdatedData { get; private set; }
    }

    [Serializable]
    public class DeleteNarrativeDecisionModel : ConvaiModel
    {
        public DeleteNarrativeDecisionModel(string apiKey, string characterID, string fromSectionId, string toSectionId, string criteria) : base(apiKey)
        {
            CharacterID = characterID;
            FromSectionId = fromSectionId;
            ToSectionId = toSectionId;
            Criteria = criteria;
        }

        [JsonProperty("character_id")]
        public string CharacterID { get; private set; }

        [JsonProperty("from_section_id")]
        public string FromSectionId { get; private set; }

        [JsonProperty("to_section_id")]
        public string ToSectionId { get; private set; }

        [JsonProperty("criteria")]
        public string Criteria { get; private set; }
    }

    [Serializable]
    public class UpdateStartNarrativeSectionModel : ConvaiModel
    {
        public UpdateStartNarrativeSectionModel(string apiKey, string characterID, string? startNarrativeSectionId) : base(apiKey)
        {
            CharacterID = characterID;
            StartNarrativeSectionId = startNarrativeSectionId;
        }

        [JsonProperty("character_id")]
        public string CharacterID { get; private set; }

        [JsonProperty("start_narrative_section_id", NullValueHandling = NullValueHandling.Include)]
        public string? StartNarrativeSectionId { get; private set; }
    }

    [Serializable]
    public class UpdateNarrativeNodePositionModel : ConvaiModel
    {
        public UpdateNarrativeNodePositionModel(string apiKey, string nodeType, JArray updatedNodes) : base(apiKey)
        {
            NodeType = nodeType;
            UpdatedNodes = updatedNodes;
        }

        [JsonProperty("node_type")]
        public string NodeType { get; private set; }

        [JsonProperty("updated_nodes")]
        public JArray UpdatedNodes { get; private set; }
    }

    [Serializable]
    public class GetCurrentNarrativeSectionModel : ConvaiModel
    {
        public GetCurrentNarrativeSectionModel(string apiKey, string characterID, string sessionID) : base(apiKey)
        {
            CharacterID = characterID;
            SessionID = sessionID;
        }

        [JsonProperty("character_id")]
        public string CharacterID { get; private set; }

        [JsonProperty("session_id")]
        public string SessionID { get; private set; }
    }
}
