using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using RiptideNetworking.Utils;
using System;

public class RigidBodyController : MonoBehaviour
{
    private static List<NetworkedObject> mapObjects = GameLogic.mapObjects;

    private static List<NetworkedObject> rigidList = new List<NetworkedObject>();

    private static bool initialized = false;


    public static void InitializeRigidObjects()
    {
        rigidList.Clear(); // Make sure the list is empty before populating

        foreach (NetworkedObject obj in mapObjects)
        {
            // Check if this object should be controlled by rigidbody physics
            if (obj.isRigid)
            {
                rigidList.Add(obj);
            }
            initialized = true;
        }

        Debug.Log($"[SERVER] RigidBodyController initialized with {rigidList.Count} rigid objects.");
    }


    private void FixedUpdate()
    {
        if (initialized)
        {
            SendRigidBodyLocations();
        }
    }


    private void SendRigidBodyLocations()
    {
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.rigidBodies);
        message.AddInt(rigidList.Count);

        try
        {
            foreach (NetworkedObject obj in rigidList)
            {
                message.AddInt(obj.id);
                message.AddVector3(obj.transform.position);
                message.AddQuaternion(obj.transform.rotation);
                message.AddInt(NetworkManager.Singleton.CurrentTick);
            }

        }
        catch (System.Exception)
        {

            throw;
        }

        // Broadcast the message to all clients.
        NetworkManager.Singleton.Server.SendToAll(message);
    }
}
