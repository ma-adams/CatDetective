using UnityEngine;

public class QuestRegistrar : MonoBehaviour
{
    public QuestData questData;
    
    void Start()
    {
        if (questData != null)
        {
            MainManager.mainManager?.RegisterQuest(questData);
        }
    }
}
