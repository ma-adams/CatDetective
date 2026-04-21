//honestly generated this with chatgpt, clips through objects and more complicated than necessary
//but works for the pickup scene's needs

using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class SimpleMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    void Update()
    {
        Vector2 input = GetMoveInput();

        Vector3 movement = new Vector3(input.x, 0f, input.y);

        transform.position += movement * moveSpeed * Time.deltaTime;

        // Keep the object upright so it never rolls or tilts
        transform.rotation = Quaternion.identity;
    }

    private Vector2 GetMoveInput()
    {
        Vector2 input = Vector2.zero;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                input.x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                input.x += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                input.y -= 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                input.y += 1f;
        }
#elif ENABLE_LEGACY_INPUT_MANAGER
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
#endif

        return input.normalized;
    }
}