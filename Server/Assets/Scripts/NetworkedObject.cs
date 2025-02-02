using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedObject : MonoBehaviour
{
    public bool isRigid;
    public int id; // Unique ID for syncing updates
    public float x, y, z; // Position
    public float rotX, rotY, rotZ; // Rotation
    public float scaleX, scaleY, scaleZ; // Scale
    public string prefabName; // The name of the object type
}
