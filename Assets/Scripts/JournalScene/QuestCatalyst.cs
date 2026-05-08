using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ADD PHYSICS RAYCASTER TO CAMERA

public class QuestCatalyst : MonoBehaviour
{
    [SerializeField] private string quest; //the quest to be added to journal, serialized for easy editing in inspector
    [SerializeField] private GameObject notification; 
    private bool questAdded = false;    //prevents duplicate quests

    public void createQuest()
    {
        if (quest != null && !questAdded)
        {
            questAdded = !questAdded;
            MainManager.mainManager.quests.Add(quest); //adds quest to main manager's list of quests
        }

        if (notification != null && !questAdded)
        {
            notification.SetActive(true); //activates notification to show quest has been added
        }
    }

    public void completeQuest()
    {
        if (quest != null && MainManager.mainManager.quests.Contains(quest))
        {
            MainManager.mainManager.quests.Remove(quest); //removes quest from main manager's list of quests
        }
    }
}
