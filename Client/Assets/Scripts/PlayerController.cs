using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using RiptideNetworking;
using RiptideNetworking.Utils;




public class PlayerController : MonoBehaviour
{

    private List<object> inputs = new List<object>();
    [SerializeField] Transform camTransform;

    private float jumpState = 0f;
    private Vector2 moveInput;
    void Start()
    {
        inputs = new List<object>();
        jumpState = 0f;
        moveInput = Vector2.zero;
    }

    void FixedUpdate()
    {
        inputs.Add(moveInput);
        inputs.Add(jumpState);
        SendInputs();
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        jumpState = ctx.ReadValue<float>();
    }

    private void SendInputs()
    {
        Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.inputs);
        for (int i = 0; i < inputs.Count; i += 3)
        {
            message.AddVector2((Vector2)inputs[i]);
            message.AddFloat((float)inputs[i + 1]);
        }
        message.AddVector3(camTransform.forward);
        NetworkManager.Singleton.Client.Send(message);
        inputs.Clear();
    }
}

