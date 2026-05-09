using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue; // default / intro dialogue
    public Dialogue questDialogue; // shown when quest is active but itm not yet completed
    public Dialogue rewardDialogue; // shown when player has the required item
    public Dialogue completedDialogue; // after quest is done, ongoing conversation
    public string requiredItem; // item needed to complete the quest
    public string questId; // quest this NPC is associated with

    public void TriggerDialogue()
    {
        Dialogue toPlay = GetDialogueBranch();
        if (toPlay == null)
        {
            Debug.LogWarning("No dialogue branch found for: " + gameObject.name);
            return;
        }
        FindFirstObjectByType<DialogueManager>().StartDialogue(GetDialogueBranch(), this);
    }

    private Dialogue GetDialogueBranch() {
        if (MainManager.mainManager == null) {
            Debug.LogWarning("MainManager not found, using default dialogue.");
            return dialogue;
        }
        bool questComplete = !string.IsNullOrEmpty(questId)
                             && MainManager.mainManager.completedQuests.Contains(questId);
        bool questActive = !string.IsNullOrEmpty(questId)
                           && MainManager.mainManager.quests.Contains(questId);
        bool hasItem = !string.IsNullOrEmpty(requiredItem)
                       && MainManager.mainManager.HasItem(requiredItem);

        if (questComplete && completedDialogue != null) return completedDialogue;
        if (questActive && hasItem && rewardDialogue != null)
            return rewardDialogue; // player has the item, complete the quest
        if (questActive && questDialogue != null)
            return questDialogue; // quest active but no item yet
        return dialogue;          // first time talking, start the quest
    }
}
