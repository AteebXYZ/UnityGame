using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using RiptideNetworking;
using RiptideNetworking.Utils;




public class PlayerController : MonoBehaviour
{
    private Vector2 inputs;
    [SerializeField] Transform camTransform;
    void Start()
    {
        inputs = Vector2.zero;
    }

    void FixedUpdate()
    {
        SendInputs();
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        inputs = ctx.ReadValue<Vector2>();
    }

    private void SendInputs()
    {
        Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.inputs);
        message.AddVector2(inputs);
        message.AddVector3(camTransform.forward);
        NetworkManager.Singleton.Client.Send(message);
    }
}
