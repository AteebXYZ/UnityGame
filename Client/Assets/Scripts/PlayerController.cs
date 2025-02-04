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
    private Vector2 scrollInput;
    void Start()
    {
        inputs = new List<object>();
        jumpState = 0f;
        moveInput = Vector2.zero;
        scrollInput = Vector2.zero;
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

    public void Scroll(InputAction.CallbackContext ctx)
    {
        var scrollInputTemp = ctx.ReadValue<Vector2>();
        scrollInput = new Vector2(scrollInputTemp.x / 120, scrollInputTemp.y / 120);
        Debug.Log(scrollInput);
        Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.sendScroll);
        message.AddVector2(scrollInput);
        NetworkManager.Singleton.Client.Send(message);
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

