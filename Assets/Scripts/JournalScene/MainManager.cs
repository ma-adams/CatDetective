using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainManager : MonoBehaviour
{
    public List<string> quests = new();
    public List<string> completedQuests = new();
    public List<string> inventory = new();
    public static MainManager mainManager;

    public System.Action onQuestsChanged;

    public void AddQuest(string questId)
    {
        if (!quests.Contains(questId))
        {
            quests.Add(questId);
            onQuestsChanged?.Invoke();
        }
    }
    
    //makes sure there is only one instace of main manager throughout game
    //can transition between scenes without losing data
    private void Awake()
    {
        if (mainManager != null)
        {
            Destroy(gameObject);
            return;
        }

        mainManager = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool HasItem(string itemId) => inventory.Contains(itemId);
    public void AddItem(string itemId) { if (!inventory.Contains(itemId)) inventory.Add(itemId); }
    public void RemoveItem(string itemId) => inventory.Remove(itemId);

    //trying to make it reload
}
