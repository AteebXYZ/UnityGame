using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

    public ushort Id { get; private set; }
    public string Username { get; private set; }

    public PlayerMovement Movement => movement;

    [SerializeField]
    private PlayerMovement movement;

    public GrabController Grab => grab;

    [SerializeField]
    private GrabController grab;


    [SerializeField]
    private Transform camTransform;

    private void OnDestroy()
    {
        list.Remove(Id);
    }




    public static void Spawn(ushort id, string username)
    {
        foreach (Player otherPlayer in list.Values)
            otherPlayer.SendSpawned(id);

        Player player = Instantiate(GameLogic.Singleton.PlayerPrefab, new Vector3(0f, 1f, 0f), Quaternion.identity).GetComponent<Player>();
        player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        player.Id = id;
        player.Username = string.IsNullOrEmpty(username) ? $"Guest {id}" : username;

        player.SendSpawned();
        list.Add(id, player);

        player.SendMapData(id);
        RigidBodyController.InitializeRigidObjects();
    }

    #region Messages
    private void SendSpawned()
    {
        NetworkManager.Singleton.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.playerSpawned)));
    }

    private void SendSpawned(ushort toClientId)
    {
        NetworkManager.Singleton.Server.Send(AddSpawnData(Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.playerSpawned)), toClientId);
    }

    private Message AddSpawnData(Message message)
    {
        message.AddUShort(Id);
        message.AddString(Username);
        message.AddVector3(transform.position);
        return message;
    }

    private void SendMapData(ushort toClientId)
    {
        List<NetworkedObject> mapObjects = GameLogic.mapObjects;
        Message message = Message.Create(MessageSendMode.reliable, ServerToClientId.mapObjects);
        message.AddInt(mapObjects.Count);
        foreach (var obj in mapObjects)
        {
            message.AddInt(obj.id);
            message.AddBool(obj.isRigid);
            message.AddString(obj.prefabName);
            message.AddFloat(obj.x);
            message.AddFloat(obj.y);
            message.AddFloat(obj.z);
            message.AddFloat(obj.rotX);
            message.AddFloat(obj.rotY);
            message.AddFloat(obj.rotZ);
            message.AddFloat(obj.scaleX);
            message.AddFloat(obj.scaleY);
            message.AddFloat(obj.scaleZ);
        }

        NetworkManager.Singleton.Server.Send(message, toClientId);
    }

    [MessageHandler((ushort)ClientToServerId.name)]
    private static void Name(ushort fromClientId, Message message)
    {
        Spawn(fromClientId, message.GetString());
    }

    [MessageHandler((ushort)ClientToServerId.inputs)]
    private static void Inputs(ushort fromClientId, Message message)
    {
        var moveInput = message.GetVector2();
        var jumpInput = message.GetFloat();
        var camForward = message.GetVector3();

        if (list.TryGetValue(fromClientId, out Player player))
        {
            player.Movement.SetInputs(moveInput, jumpInput, camForward);
        }
    }

    [MessageHandler((ushort)ClientToServerId.scroll)]
    private static void SetScrollInput(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out Player player))
        {
            player.grab.Scroll(message.GetVector2());
        }
    }


    [MessageHandler((ushort)ClientToServerId.sendRotate)]
    private static void SetRotateInput(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out Player player))
        {
            player.grab.Rotate(message.GetFloat());
        }
    }

    [MessageHandler((ushort)ClientToServerId.sendRotateVector)]
    private static void SetRotateVectorInput(ushort fromClientId, Message message)
    {
        if (list.TryGetValue(fromClientId, out Player player))
        {
            player.grab.RotateVector(message.GetVector2());
        }
    }
    #endregion
}