using UnityEngine;
using UnityEngine.InputSystem;

public class ConditionalPickup : MonoBehaviour
{
    [SerializeField] private string itemId;
    [SerializeField] private string[] requiredCompletedQuests;

    private bool isVisible = false;
    private Renderer[] renderers;
    private Collider[] colliders;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();
        SetVisible(false);
    }

    void SetVisible(bool visible)
    {
        foreach (Renderer r in renderers) r.enabled = visible;
        foreach (Collider c in colliders) c.enabled = visible;
        Outline outline = GetComponent<Outline>();
        if (outline != null) outline.enabled = visible;
    }

    void Update()
    {
        if (!isVisible && MainManager.mainManager != null)
        {
            if (MainManager.mainManager.AllQuestsComplete(requiredCompletedQuests))
            {
                SetVisible(true);
                isVisible = true;
                Debug.Log(itemId + " has appeared!");
            }
        }
    }

    public void Pickup()
    {
        if (!string.IsNullOrEmpty(itemId))
            MainManager.mainManager.AddItem(itemId);
        SetVisible(false);
    }
}
