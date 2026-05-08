using UnityEngine;
using UnityEngine.InputSystem;

// Requires a Rigidbody and Collider on the cat GameObject.
// In the Rigidbody inspector: freeze X and Z rotation so the cat stays upright.
[RequireComponent(typeof(Rigidbody))]
public class CatMovementPhysics : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotationSpeed = 360f;
    public ThirdPersonShooterCamera playerCamera;

    private Rigidbody rb;
    private Animator animator;
    private Transform cachedCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (playerCamera != null)
            cachedCamera = playerCamera.transform;
        else if (Camera.main != null)
            cachedCamera = Camera.main.transform;
        else
            Debug.LogError("CatMovementPhysics: no camera found. Assign Player Camera in the Inspector.");
    }

    void FixedUpdate()
    {
        if (cachedCamera == null) return;

        Vector2 input = new Vector2(
            Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1 :
            Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1 : 0,
            Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1 :
            Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? -1 : 0
        );

        if (input.sqrMagnitude > 0)
        {
            float cameraYaw = playerCamera != null
                ? playerCamera.Yaw
                : cachedCamera.eulerAngles.y;

            Quaternion camYawRotation = Quaternion.Euler(0f, cameraYaw, 0f);
            Vector3 moveDir = (camYawRotation * new Vector3(input.x, 0f, input.y)).normalized;

            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.RotateTowards(
                rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime
            ));

            if (animator != null) animator.SetBool("isWalking", true);
        }
        else
        {
            if (animator != null) animator.SetBool("isWalking", false);
        }
    }
}
