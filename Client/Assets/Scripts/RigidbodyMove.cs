using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using RiptideNetworking.Utils;

public class RigidbodyMove : MonoBehaviour
{
    private static int rigidCount;

    [MessageHandler((ushort)ServerToClientId.rigidBodies)]
    private static void UpdateRigidbody(Message message)
    {
        rigidCount = message.GetInt();

        for (int i = 0; i < rigidCount; i++)
        {
            int id = message.GetInt();
            Vector3 newPosition = message.GetVector3();
            Quaternion newRotation = message.GetQuaternion();

            foreach (NetworkedObject obj in FindObjectsOfType<NetworkedObject>())
            {
                if (obj.id == id)
                {
                    RigidbodyInterpolator interpolator = obj.GetComponent<RigidbodyInterpolator>();
                    if (interpolator != null)
                    {
                        interpolator.NewUpdate(message.GetInt(), newPosition, newRotation);
                    }
                    break;
                }
            }
        }
    }
}
