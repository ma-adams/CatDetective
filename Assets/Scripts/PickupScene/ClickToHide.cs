using UnityEngine;
using UnityEngine.InputSystem;

public class ClickToHide : MonoBehaviour
{
    public Transform player;
    public Transform npc;
    public float detectionRadius = 3f;

    [SerializeField] private string itemId;
    [SerializeField] private string requiredQuestId;

    public bool IsInteractable()
    {
        if (string.IsNullOrEmpty(requiredQuestId)) return true;
        return MainManager.mainManager != null && MainManager.mainManager.quests.Contains(requiredQuestId);
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
        if (!string.IsNullOrEmpty(itemId)) {
            MainManager.mainManager.AddItem(itemId);
        }
        gameObject.SetActive(false);
    }
}