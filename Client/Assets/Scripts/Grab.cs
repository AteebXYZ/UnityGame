using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using RiptideNetworking;
using RiptideNetworking.Utils;

public class Grab : MonoBehaviour

{
    private bool pressing;
    private bool down;

    private void Update()
    {
        // if (pressing == true)
        // {
        //     SendGrab();
        // }

    }

    private void SendGrab(bool boolean)
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.sendGrab);
        message.AddBool(boolean);
        NetworkManager.Singleton.Client.Send(message);
    }
    public void GrabObject(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            SendGrab(true);
        }
    }
}
