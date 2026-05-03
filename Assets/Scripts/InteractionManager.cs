using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    public float detectionRadius = 3f;

    void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.eKey.wasPressedThisFrame)
            return;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);

        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        foreach (Collider col in hits)
        {
            ClickToHide interactable = col.GetComponent<ClickToHide>();
            if (interactable == null) continue;

            Outline outline = col.GetComponent<Outline>();
            if (outline == null || !outline.enabled) continue;

            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = col.gameObject;
            }
        }

        if (closest != null)
        {
            closest.GetComponent<DialogueTrigger>()?.TriggerDialogue();
            closest.SetActive(false);
        }
    }
}
