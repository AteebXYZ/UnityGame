using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{

    [SerializeField] private float sensitivity = 10f;
    [SerializeField] private GameObject player;

    private Vector2 lookInput;
    private float xRot = 0f;
    private float yRot = 0f;

    private void Start()
    {
        // Lock and hide the cursor for an FPS-like experience
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Update rotation values based on input
        // (Divide sensitivity by 10 to match your original scale, adjust as needed)
        xRot += lookInput.y * (sensitivity / 10f);
        yRot += lookInput.x * (sensitivity / 10f);

        // Clamp the vertical rotation so the camera doesn't flip over
        xRot = Mathf.Clamp(xRot, -80f, 80f);

        // Apply the rotations:
        // Rotate the camera for vertical movement (pitch)


        transform.localRotation = Quaternion.Euler(-xRot, 0f, 0f);


        // Rotate the player GameObject for horizontal movement (yaw)
        player.transform.localRotation = Quaternion.Euler(0f, yRot, 0f);
    }

    // This method will be called by the Input System when look input is received
    public void Look(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
}
