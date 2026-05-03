using UnityEngine;
using UnityEngine.InputSystem;

public class ClickToHide : MonoBehaviour
{
    public Transform player;
    public Transform npc;
    public float detectionRadius = 3f;
    private bool isPlayerNearby = false;

    void Update()
    {
        // check distance between player and npc
        float distance = Vector3.Distance(player.position, npc.position);
        isPlayerNearby = distance <= detectionRadius;

        // mouse click
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform && hit.transform.GetComponent<Outline>().enabled == true)
                {
                    hit.transform.GetComponent<DialogueTrigger>().TriggerDialogue();
                    gameObject.SetActive(false);
                }
            }
        }

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame && isPlayerNearby) 
        {
            Outline outline = GetComponent<Outline>();
            if (outline != null && outline.enabled)
            {
                GetComponent<DialogueTrigger>()?.TriggerDialogue();
                gameObject.SetActive(false);
            }
        }
    }
}