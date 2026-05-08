using UnityEngine;

public class ThirdPersonCameraFollow : MonoBehaviour
{
    public Transform target;

    public float distance = 5f;
    public float height = 3f;
    public float followSpeed = 10f;
    public float rotationSpeed = 10f;

    void LateUpdate()
    {
        if (target == null) return;

        // Desired position: behind the cat based on its facing direction
        Vector3 desiredPosition = target.position
            - target.forward * distance
            + Vector3.up * height;

        // Smoothly move to desired position
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            Time.deltaTime * followSpeed
        );

        // Smoothly look at the cat
        Quaternion desiredRotation = Quaternion.LookRotation(
            target.position - transform.position
        );
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            Time.deltaTime * rotationSpeed
        );
    }
}
