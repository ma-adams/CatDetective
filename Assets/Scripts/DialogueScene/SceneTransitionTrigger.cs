using UnityEngine;
using UnityEngine.InputSystem;

public class SceneTransitionTrigger : MonoBehaviour
{
    public string requiredItem;
    public string nextScene;

    private bool playerNearby = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = true;
        Debug.Log("Player entered door trigger.");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = false;
        Debug.Log("Player exited door trigger.");
    }

    private void Update()
    {
        if (!playerNearby) return;

        if (Keyboard.current == null || !Keyboard.current.eKey.wasPressedThisFrame)
            return;

        if (MainManager.mainManager.HasItem(requiredItem))
        {
            Debug.Log("Key found. Loading scene: " + nextScene);
            MainManager.mainManager.LoadScene(nextScene);
        }
        else
        {
            Debug.Log("Door is locked. Find the key first.");
        }
    }
}