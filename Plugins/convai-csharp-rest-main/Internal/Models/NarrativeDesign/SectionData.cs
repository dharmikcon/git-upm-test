using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Convai.RestAPI.Internal.Models
{
    /// <summary>
    /// Represents a narrative design section with its associated data.
    /// </summary>
    [Serializable]
    public class SectionData
    {
        /// <summary>
        /// Initializes a new instance of the SectionData class.
        /// </summary>
        [JsonConstructor]
        public SectionData(string sectionID, string sectionName, JToken? behaviorTreeConstants, string objective, string characterId, object decisions, object parents, object triggers, object updatedCharacterData, object? nodePosition = null, string? behaviorTreeCode = null)
        {
            SectionId = sectionID;
            SectionName = sectionName;
            BehaviorTreeConstants = behaviorTreeConstants;
            Objective = objective;
            CharacterId = characterId;
            Decisions = decisions;
            Parents = parents;
            Triggers = triggers;
            UpdatedCharacterData = updatedCharacterData;
            NodePosition = nodePosition;
            BehaviorTreeCode = behaviorTreeCode;
        }

        /// <summary>
        /// The unique identifier for the section.
        /// </summary>
        [JsonProperty("section_id")]
        public string SectionId { get; internal set; }

        /// <summary>
        /// The name of the section.
        /// </summary>
        [JsonProperty("section_name")]
        public string SectionName { get; internal set; }

        /// <summary>
        /// Constants used in the behavior tree for this section.
        /// </summary>
        [JsonProperty("bt_constants")]
        public JToken? BehaviorTreeConstants { get; internal set; }

        /// <summary>
        /// The objective or goal of this narrative section.
        /// </summary>
        [JsonProperty("objective")]
        public string Objective { get; internal set; }

        /// <summary>
        /// The ID of the character associated with this section.
        /// </summary>
        [JsonProperty("character_id")]
        public string CharacterId { get; internal set; }

        /// <summary>
        /// Decisions that can be made within this section.
        /// </summary>
        [JsonProperty("decisions")]
        public object Decisions { get; internal set; }

        /// <summary>
        /// Parent sections that lead to this section.
        /// </summary>
        [JsonProperty("parents")]
        public object Parents { get; internal set; }

        /// <summary>
        /// Triggers that can be activated in this section.
        /// </summary>
        [JsonProperty("triggers")]
        public object Triggers { get; internal set; }

        /// <summary>
        /// Updated character data after section completion.
        /// </summary>
        [JsonProperty("updated_character_data")]
        public object UpdatedCharacterData { get; internal set; }

        /// <summary>
        /// The stored node position for this section in the editor.
        /// </summary>
        [JsonProperty("node_position")]
        public object? NodePosition { get; internal set; }

        /// <summary>
        /// Serialized behavior tree code associated with this section.
        /// </summary>
        [JsonProperty("behavior_tree_code")]
        public string? BehaviorTreeCode { get; internal set; }
    }
}
