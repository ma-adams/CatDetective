using UnityEngine;
using UnityEngine.InputSystem;

public class ClickToHide : MonoBehaviour
{
    public Transform player;
    public Transform npc;
    public float detectionRadius = 3f;

    [SerializeField] private string itemId;
    [SerializeField] private string requiredQuestId;
    [SerializeField] private string completeQuestId;
    [SerializeField] private string startQuestId;
    [SerializeField] private string[] requiredCompletedQuests; // for door outline

    public bool IsInteractable()
    {
        if (string.IsNullOrEmpty(requiredQuestId) && (requiredCompletedQuests == null || requiredCompletedQuests.Length == 0)) 
            return true;
        
        if (!string.IsNullOrEmpty(requiredQuestId))
            return MainManager.mainManager != null && MainManager.mainManager.quests.Contains(requiredQuestId);
        
        return MainManager.mainManager != null && MainManager.mainManager.AllQuestsComplete(requiredCompletedQuests);
    }

    void Update()
    {
        // mouse click
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            return;
    
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform && hit.transform.GetComponent<Outline>().enabled == true)
            {
                PickUp();
            }
        }
    }

    public void PickUp() {
        if (!string.IsNullOrEmpty(itemId))
            MainManager.mainManager.AddItem(itemId);

        if (!string.IsNullOrEmpty(completeQuestId))
        {
            MainManager.mainManager.quests.Remove(completeQuestId);
            if (!MainManager.mainManager.completedQuests.Contains(completeQuestId))
                MainManager.mainManager.completedQuests.Add(completeQuestId);
            MainManager.mainManager.onQuestsChanged?.Invoke();
        }

        if (!string.IsNullOrEmpty(startQuestId))
            MainManager.mainManager.AddQuest(startQuestId);

        gameObject.SetActive(false);
    }
}