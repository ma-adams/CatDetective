using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonShooterCamera : MonoBehaviour
{
    public Transform target;

    public float distance = 5f;
    public float height = 1.5f;
    public float mouseSensitivity = 3f;
    public float pitchMin = -20f;
    public float pitchMax = 60f;

    private float yaw;
    private float pitch = 15f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize yaw to current camera facing so it doesn't snap on start
        yaw = transform.eulerAngles.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        yaw += mouseDelta.x * mouseSensitivity * Time.deltaTime;
        pitch -= mouseDelta.y * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = target.position + rotation * new Vector3(0f, height, -distance);

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * height);
    }

    // Expose yaw so the movement script can read camera horizontal direction
    public float Yaw => yaw;
}
