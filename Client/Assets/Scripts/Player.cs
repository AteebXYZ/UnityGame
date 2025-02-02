using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

    public ushort Id { get; private set; }
    public bool IsLocal { get; private set; }

    [SerializeField]
    private Transform camTransform;
    private string username;

    private void OnDestroy()
    {
        list.Remove(Id);
    }

    private void Move(Vector3 newPosition, Vector3 forward)
    {
        transform.position = newPosition;
        if (!IsLocal)
        {
            camTransform.forward = forward;
        }
    }

    public static void Spawn(ushort id, string username, Vector3 position)
    {
        Player player;
        if (id == NetworkManager.Singleton.Client.Id)
        {
            player = Instantiate(GameLogic.Singleton.LocalPlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = true;
        }
        else
        {
            player = Instantiate(GameLogic.Singleton.PlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = false;
        }

        player.name = $"Player {id} (username)";
        player.Id = id;
        player.username = username;

        list.Add(id, player);
    }

    [MessageHandler((ushort)ServerToClientId.playerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector3());
    }

    [MessageHandler((ushort)ServerToClientId.playerMovement)]
    private static void PlayerMovement(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
        {
            player.Move(message.GetVector3(), message.GetVector3());
        }
    }

    [MessageHandler((ushort)ServerToClientId.mapObjects)]
    private static void SpawnMapObjects(Message message)
    {
        int objectCount = message.GetInt();
        for (int i = 0; i < objectCount; i++)
        {
            int id = message.GetInt();
            bool isRigid = message.GetBool();
            string prefabName = message.GetString();
            float x = message.GetFloat();
            float y = message.GetFloat();
            float z = message.GetFloat();
            float rx = message.GetFloat();
            float ry = message.GetFloat();
            float rz = message.GetFloat();
            float sx = message.GetFloat();
            float sy = message.GetFloat();
            float sz = message.GetFloat();

            GameObject prefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");
            if (prefab != null)
            {
                GameObject obj = Instantiate(prefab, new Vector3(x, y, z), Quaternion.Euler(rx, ry, rz));
                obj.transform.localScale = new Vector3(sx, sy, sz);
                obj.GetComponent<NetworkedObject>().id = id;
            }
            else
            {
                Debug.Log("Missing prefab: " + prefabName);
            }
        }
    }
}