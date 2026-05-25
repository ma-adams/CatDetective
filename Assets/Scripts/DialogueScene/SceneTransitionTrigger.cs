using UnityEngine;
using UnityEngine.InputSystem;

public class SceneTransitionTrigger : MonoBehaviour
{
    public string requiredItem;
    public string nextScene;
    public GameObject lockedPrompt;
    public GameObject unlockedPrompt;

    private bool playerNearby = false;

    void Start()
    {
        if (lockedPrompt != null) lockedPrompt.SetActive(false);
        if (unlockedPrompt != null) unlockedPrompt.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = true;

        if (MainManager.mainManager.HasItem(requiredItem))
            unlockedPrompt?.SetActive(true);
        else
            lockedPrompt?.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = false;
        lockedPrompt?.SetActive(false);
        unlockedPrompt?.SetActive(false);
    }

    void Update()
    {
        if (!playerNearby) return;
        if (Keyboard.current == null || !Keyboard.current.eKey.wasPressedThisFrame) return;

        if (MainManager.mainManager.HasItem(requiredItem))
            MainManager.mainManager.LoadScene(nextScene);
        else
            Debug.Log("Door is locked. Find the key first.");
    }
}