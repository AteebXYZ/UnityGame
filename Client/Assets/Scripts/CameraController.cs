using RiptideNetworking;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float sensitivity = 10f;
    [SerializeField] private GameObject player;

    private Vector2 lookInput;
    private float xRot = 0f;
    private float yRot = 0f;
    private float rotateInput;
    private static bool isGrabbing;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {

        if (rotateInput == 1f && isGrabbing == true)
        {
            return;
        }

        xRot += lookInput.y * (sensitivity / 10f);
        yRot += lookInput.x * (sensitivity / 10f);

        xRot = Mathf.Clamp(xRot, -80f, 80f);

        transform.localRotation = Quaternion.Euler(-xRot, 0f, 0f);

        player.transform.localRotation = Quaternion.Euler(0f, yRot, 0f);
    }

    public void Look(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void RotateCheck(InputAction.CallbackContext ctx)
    {
        rotateInput = ctx.ReadValue<float>();
        if (rotateInput != 0f)
        {
            rotateInput = 1f;
        }

    }

    [MessageHandler((ushort)ServerToClientId.isGrabbing)]
    private static void GrabCheck(Message message)
    {
        isGrabbing = message.GetBool();
    }
}

