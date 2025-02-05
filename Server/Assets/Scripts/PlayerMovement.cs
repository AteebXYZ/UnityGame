using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using RiptideNetworking.Utils;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Transform camProxy;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private Rigidbody rb;

    private Vector2 playerInputs;
    private Vector3 moveVector;
    private float jumpState;
    private void OnValidate()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    private void Start()
    {
        playerInputs = Vector2.zero;
    }

    private void FixedUpdate()
    {
        Move();
        Jump();
        SendMovement();
        // Debug.DrawRay(camProxy.position, camProxy.forward * 1.5f, Color.green);

    }

    public void Move()
    {
        Vector3 forward = camProxy.forward;
        forward.y = 0;
        forward = forward.normalized;
        moveVector = forward * playerInputs.y * movementSpeed;
        moveVector += camProxy.right * playerInputs.x * movementSpeed;
        rb.velocity = new Vector3(moveVector.x, rb.velocity.y, moveVector.z);
    }

    private void Jump()
    {
        if (IsGrounded() && jumpState > 0)
        {
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);

        }
    }

    private bool IsGrounded()
    {
        var groundCheckDistance = GetComponent<CapsuleCollider>().height / 2 + 0.1f;
        Ray ray = new Ray(transform.position, -transform.up);
        // Debug.DrawRay(transform.position, -transform.up, Color.blue);
        Debug.DrawRay(transform.position, -transform.up, Color.blue, groundCheckDistance);

        if (Physics.Raycast(ray, groundCheckDistance))
        {
            //Debug.Log(hitInfo.collider);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetInputs(Vector2 inputs, float jumpState, Vector3 forward)
    {
        playerInputs = inputs;
        this.jumpState = jumpState;
        camProxy.forward = forward;
    }

    private void SendMovement()
    {

        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.playerMovement);
        message.AddUShort(player.Id);
        message.AddInt(NetworkManager.Singleton.CurrentTick);
        message.AddVector3(transform.position);
        message.AddQuaternion(transform.rotation);
        message.AddVector3(camProxy.forward);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
}
