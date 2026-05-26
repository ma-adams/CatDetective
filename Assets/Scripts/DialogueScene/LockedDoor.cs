using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [SerializeField] private string unlockedByQuest;
    // Optional: list specific child objects to disable (mesh + blocker collider).
    // If left empty, all Colliders and Renderers on this GameObject are disabled.
    [SerializeField] private GameObject[] barrierObjects;

    private bool isOpen = false;

    void Update()
    {
        if (!isOpen && MainManager.mainManager != null
            && MainManager.mainManager.completedQuests.Contains(unlockedByQuest))
            OpenDoor();
    }

    void OpenDoor()
    {
        isOpen = true;
        if (barrierObjects != null && barrierObjects.Length > 0)
        {
            foreach (var obj in barrierObjects)
                if (obj != null) obj.SetActive(false);
        }
        else
        {
            foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;
            foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = false;
        }
    }
}
