using UnityEngine;
using UnityEngine.InputSystem;

public class NPCProximity : MonoBehaviour
{
    public Transform player;
    public Transform npc;
    public float detectionRadius = 3f;
    private bool isPlayerNearby = false;
    public GameObject interactPrompt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactPrompt.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // check distance between player and npc
        float distance = Vector3.Distance(player.position, npc.position);
        // if player is close enough, trigger prompt to start dialogue
        if (distance <= detectionRadius)
        {
            if (!isPlayerNearby)
            {
                isPlayerNearby = true;
            }

            // Only show the "E to interact" prompt if the pickup is currently interactable
            // (e.g. its required quest has been triggered). Updates live as quest state changes.
            ClickToHide pickup = GetComponent<ClickToHide>();
            bool interactable = pickup == null || pickup.IsInteractable();
            interactPrompt.SetActive(interactable);
        } else {
            if (isPlayerNearby)
            {
                isPlayerNearby = false;
                interactPrompt.SetActive(false);
                if (DialogueManager.IsOpen)
                    FindFirstObjectByType<DialogueManager>()?.ForceClose();
            }
        }
        
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame && isPlayerNearby && !DialogueManager.IsOpen) {
            GetComponent<DialogueTrigger>()?.TriggerDialogue();
        }

    }
}
