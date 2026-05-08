using UnityEngine;
using UnityEngine.InputSystem;

public class CatMovementCameraRelative : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotationSpeed = 360f;
    public Transform cameraTransform;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        Vector2 input = new Vector2(
            Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1 :
            Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1 : 0,
            Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1 :
            Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? -1 : 0
        );

        if (input.sqrMagnitude > 0)
        {
            // Flatten camera axes onto XZ plane so vertical camera angle doesn't affect speed
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Vector3 camRight = cameraTransform.right;
            camRight.y = 0f;
            camRight.Normalize();

            Vector3 moveDir = (camForward * input.y + camRight * input.x).normalized;

            transform.position += moveDir * moveSpeed * Time.deltaTime;

            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRotation, rotationSpeed * Time.deltaTime
            );

            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
}
