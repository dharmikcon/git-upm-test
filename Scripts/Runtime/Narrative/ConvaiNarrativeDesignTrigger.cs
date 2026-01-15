using Convai.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class ConvaiNarrativeDesignTrigger : MonoBehaviour
{
    [field: SerializeField] public string TriggerName { get; private set; }
    [field: SerializeField] public string TriggerMessage { get; private set; }
    [field: SerializeField] public ConvaiNPC NPC { get; private set; }
    [field: SerializeField] public UnityEvent OnTriggerInvoked { get; private set; }


    public void InvokeTrigger()
    {
        if (NPC == null)
        {
            Debug.LogError("[Unity Engine] [Narrative Design] ConvaiNarrativeDesignTrigger: NPC is not set");
            return;
        }

        if (!string.IsNullOrEmpty(TriggerName))
        {
            NPC.SendTriggerEvent(TriggerName);
        }
        else
        {
            NPC.SendTriggerEvent("", TriggerMessage);
        }

        OnTriggerInvoked.Invoke();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NPC.SendTriggerEvent(TriggerName);
        }
    }


    public void SetNPC(ConvaiNPC npc) => NPC = npc;

    public void SetTriggerName(string triggerName) => TriggerName = triggerName;

    public void SetTriggerMessage(string triggerMessage) => TriggerMessage = triggerMessage;
}
