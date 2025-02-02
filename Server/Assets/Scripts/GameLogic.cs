using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public static List<NetworkedObject> mapObjects = new List<NetworkedObject>();

    private static GameLogic _singleton;
    public static GameLogic Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(GameLogic)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    public GameObject PlayerPrefab => playerPrefab;

    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;

    private void Awake()
    {
        Singleton = this;
        PopulateMapObjects();
    }

    private void PopulateMapObjects()
    {
        mapObjects.Clear();
        NetworkedObject[] objects = FindObjectsOfType<NetworkedObject>();
        int idCounter = 1;
        foreach (NetworkedObject obj in objects)
        {
            obj.id = idCounter++;
            if (obj.GetComponent<Rigidbody>() != null)
            {
                obj.isRigid = true;
            }
            else
            {
                obj.isRigid = false;
            }
            obj.prefabName = obj.name;
            obj.x = obj.transform.position.x;
            obj.y = obj.transform.position.y;
            obj.z = obj.transform.position.z;
            obj.rotX = obj.transform.rotation.x;
            obj.rotY = obj.transform.rotation.y;
            obj.rotZ = obj.transform.rotation.z;
            obj.scaleX = obj.transform.localScale.x;
            obj.scaleY = obj.transform.localScale.y;
            obj.scaleZ = obj.transform.localScale.z;
            mapObjects.Add(obj);
        }
    }
}