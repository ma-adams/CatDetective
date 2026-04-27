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
                interactPrompt.SetActive(true);
            }
        } else {
            if (isPlayerNearby)
            {
                isPlayerNearby = false;
                interactPrompt.SetActive(false);
            }
        }
        
        if (Keyboard.current.eKey.wasPressedThisFrame && isPlayerNearby) {
            GetComponent<DialogueTrigger>().TriggerDialogue();
        }

    }
}
