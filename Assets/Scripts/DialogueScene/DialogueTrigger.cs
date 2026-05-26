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
    public string[] requiredItems; // multiple items needed to complete the quest
    public string[] requiredCompletedQuests; // quests that must be completed before reward dialogue
    public string questId; // quest this NPC is associated with

    public void TriggerDialogue()
    {
        Dialogue toPlay = GetDialogueBranch();
        if (toPlay == null)
        {
            Debug.LogWarning("No dialogue branch found for: " + gameObject.name);
            return;
        }
        FindFirstObjectByType<DialogueManager>().StartDialogue(toPlay, this);
    }

    private Dialogue GetDialogueBranch() {
        if (MainManager.mainManager == null) {
            return dialogue;
        }
        bool questComplete = !string.IsNullOrEmpty(questId)
                             && MainManager.mainManager.completedQuests.Contains(questId);
        bool questActive = !string.IsNullOrEmpty(questId)
                           && MainManager.mainManager.quests.Contains(questId);
        bool hasAllItems = HasRequiredItems();
        bool hasCompletedPrereqs = HasRequiredCompletedQuests();

        if (questComplete && completedDialogue != null) return completedDialogue;
        if (questActive && hasAllItems && hasCompletedPrereqs && rewardDialogue != null)
            return rewardDialogue;
        if (questActive && questDialogue != null)
            return questDialogue;

        return dialogue;
    }

    private bool HasRequiredItems()
    {
        // Multi-item quest
        if (requiredItems != null && requiredItems.Length > 0)
        {
            foreach (string item in requiredItems)
                if (!MainManager.mainManager.HasItem(item))
                    return false;
            return true;
        }

        // Single item quest (existing behavior)
        if (!string.IsNullOrEmpty(requiredItem))
            return MainManager.mainManager.HasItem(requiredItem);

        return true; // no item required
    }

    private bool HasRequiredCompletedQuests()
    {
        if (requiredCompletedQuests == null || requiredCompletedQuests.Length == 0) return true;
        return MainManager.mainManager.AllQuestsComplete(requiredCompletedQuests);
    }
}
