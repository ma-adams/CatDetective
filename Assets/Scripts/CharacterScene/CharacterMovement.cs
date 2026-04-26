using UnityEngine;
using UnityEngine.InputSystem;

public class CatMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotationSpeed = 360f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 input = new Vector2(
            Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1 : 
            Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1 : 0,
            Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1 : 
            Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? -1 : 0
        );

        Vector3 movement = new Vector3(input.x, 0f, input.y);

        if (movement.magnitude > 0) // if > 0, key is being pressed, character is walking
        {
            transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
            Quaternion targetRotation = Quaternion.LookRotation(movement); // calculates necessary cat rotation
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // actually rotates
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
}