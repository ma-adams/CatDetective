using UnityEngine;
using UnityEngine.InputSystem;

public class ClickToHide : MonoBehaviour
{
    public Transform player;
    public Transform npc;
    public float detectionRadius = 3f;

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
                hit.transform.GetComponent<DialogueTrigger>().TriggerDialogue();
                gameObject.SetActive(false);
            }
        }
    }
}