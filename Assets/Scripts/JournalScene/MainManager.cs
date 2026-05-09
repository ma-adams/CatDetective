using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainManager : MonoBehaviour
{

    public static MainManager mainManager;

    public List<string> quests = new();
    public List<string> completedQuests = new();
    public List<string> inventory = new();

    private Dictionary<string, QuestData> questRegistry = new();

    public System.Action onQuestsChanged;

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

    public void RegisterQuest(QuestData data)
    {
        if (!questRegistry.ContainsKey(data.questId))
        {
            questRegistry[data.questId] = data;
        }
    }

    public QuestData GetQuestData(string questId) 
    {
        questRegistry.TryGetValue(questId, out QuestData data);
        return data;
    }    

    public void AddQuest(string questId)
    {
        if (!quests.Contains(questId))
        {
            quests.Add(questId);
            onQuestsChanged?.Invoke();
        }
    }

    public bool HasItem(string itemId) => inventory.Contains(itemId);
    public void AddItem(string itemId) { if (!inventory.Contains(itemId)) inventory.Add(itemId); }
    public void RemoveItem(string itemId) => inventory.Remove(itemId);

    //trying to make it reload
}
