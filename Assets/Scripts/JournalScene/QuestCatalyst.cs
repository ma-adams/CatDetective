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
            MainManager.mainManager.quests.Add(questId); //adds quest to main manager's list of quests
            if (notification != null)
                notification.SetActive(true);
        }
    }

    public void completeQuest()
    {
        if (MainManager.mainManager.quests.Contains(questId))
        {
            MainManager.mainManager.quests.Remove(questId);
            if (!MainManager.mainManager.completedQuests.Contains(questId))
                MainManager.mainManager.completedQuests.Add(questId);
        }
    }
}
