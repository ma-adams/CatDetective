using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ADD PHYSICS RAYCASTER TO CAMERA

public class QuestCatalyst : MonoBehaviour
{
    [SerializeField] private string questId; //the quest to be added to journal, serialized for easy editing in inspector
    [SerializeField] private GameObject notification; 
    private bool questAdded = false;    //prevents duplicate quests

    public void createQuest()
    {
        if (questId != null && !questAdded)
        {
            questAdded = true;
            // Route through AddQuest so onQuestAdded fires (and the start SFX plays).
            // The journal scripts handle lighting the notification dot via that event,
            // so we no longer touch `notification` directly here.
            MainManager.mainManager.AddQuest(questId);
        }
    }

    public void completeQuest()
    {
        if (MainManager.mainManager.quests.Contains(questId))
        {
            MainManager.mainManager.quests.Remove(questId);
            MainManager.mainManager.CompleteQuest(questId);
        }
    }
}
