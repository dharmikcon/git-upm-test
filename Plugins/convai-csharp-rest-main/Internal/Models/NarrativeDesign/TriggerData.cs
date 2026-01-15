using System;
using Newtonsoft.Json;

namespace Convai.RestAPI.Internal.Models
{
    /// <summary>
    /// Represents a narrative design trigger with its associated data.
    /// </summary>
    [Serializable]
    public class TriggerData
    {
        /// <summary>
        /// Initializes a new instance of the TriggerData class.
        /// </summary>
        [JsonConstructor]
        public TriggerData(string id, string name, string message, string destinationSectionID, string characterId, object? nodePosition = null)
        {
            TriggerId = id;
            TriggerName = name;
            TriggerMessage = message;
            DestinationSection = destinationSectionID;
            CharacterId = characterId;
            NodePosition = nodePosition;
        }

        /// <summary>
        /// The unique identifier for the trigger.
        /// </summary>
        [JsonProperty("trigger_id")]
        public string TriggerId { get; internal set; }

        /// <summary>
        /// The name of the trigger.
        /// </summary>
        [JsonProperty("trigger_name")]
        public string TriggerName { get; internal set; }

        /// <summary>
        /// The message associated with this trigger.
        /// </summary>
        [JsonProperty("trigger_message")]
        public string TriggerMessage { get; internal set; }

        /// <summary>
        /// The ID of the section this trigger leads to.
        /// </summary>
        [JsonProperty("destination_section")]
        public string DestinationSection { get; internal set; }

        /// <summary>
        /// The ID of the character associated with this trigger.
        /// </summary>
        [JsonProperty("character_id")]
        public string CharacterId { get; internal set; }

        /// <summary>
        /// The saved node position for layout purposes.
        /// </summary>
        [JsonProperty("node_position")]
        public object? NodePosition { get; internal set; }
    }
}
