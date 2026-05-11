using UnityEngine;
using UnityEngine.InputSystem;

public class ConditionalPickup : MonoBehaviour
{
    [SerializeField] private string itemId;
    [SerializeField] private string[] requiredCompletedQuests; // all must be done to appear

    private bool isVisible = false;

    void Start()
    {
        gameObject.SetActive(false); // hidden by default
    }

    void Update()
    {
        if (!isVisible && MainManager.mainManager != null)
        {
            if (MainManager.mainManager.AllQuestsComplete(requiredCompletedQuests))
            {
                gameObject.SetActive(true);
                isVisible = true;
                Debug.Log(itemId + " has appeared!");
            }
        }

        if (isVisible && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform && hit.transform.GetComponent<Outline>().enabled)
                    Pickup();
            }
        }
    }

    public void Pickup()
    {
        if (!string.IsNullOrEmpty(itemId))
            MainManager.mainManager.AddItem(itemId);
        gameObject.SetActive(false);
    }
}