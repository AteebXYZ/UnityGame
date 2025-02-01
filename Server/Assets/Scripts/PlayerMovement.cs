using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    private Vector2 playerInputs;
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
    }

    public void Move()
    {
        rb.velocity = new Vector3(playerInputs.x, rb.velocity.y, playerInputs.y);
    }

    public void SetInputs(Vector2 inputs)
    {
        playerInputs = inputs;
    }
}
