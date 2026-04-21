using UnityEngine;
using UnityEngine.InputSystem;

public class ClickToHide : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                gameObject.SetActive(false);
            }
        }
    }
}