using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using RiptideNetworking.Utils;

public class GrabController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject cam;     // Player's camera
    [SerializeField] private GameObject holder;   // The transform where grabbed objects should go
    [SerializeField] private float minGrabRange = 3f;
    [SerializeField] private float maxGrabRange = 6f;

    [Header("Settings")]
    [SerializeField] private float forceMagnitude = 10f;

    // Instance variables to track grabbing state for each player
    private bool isGrabbing = false;
    private GameObject grabbedObject = null;
    private bool isPlayer;
    private Vector2 scrollInput;

    // This method is called every frame on each player's instance
    private void Update()
    {
        if (isGrabbing && grabbedObject != null)
        {
            // Calculate direction and distance from the grabbed object to the holder
            Vector3 direction = holder.transform.position - grabbedObject.transform.position;
            float distance = Vector3.Distance(holder.transform.position, grabbedObject.transform.position);

            if (distance > 0.2f)
            {
                Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    if (!isPlayer)
                    {

                        rb.drag = 10;
                        rb.freezeRotation = true;
                        rb.useGravity = false;
                        rb.AddForce(direction * forceMagnitude, ForceMode.Force);

                    }
                    if (isPlayer)
                    {
                        rb.drag = 3;
                        rb.freezeRotation = true;
                        rb.useGravity = false;
                        rb.AddForce(direction * forceMagnitude, ForceMode.Force);
                    }
                }
            }
        }
        else if (grabbedObject != null)
        {
            // Release the object and restore its physics
            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
            if (rb != null && grabbedObject.GetComponent<RigidVariables>().beingHeld && !isPlayer)
            {
                rb.drag = 0;
                rb.freezeRotation = false;
                rb.useGravity = true;
                grabbedObject.GetComponent<RigidVariables>().beingHeld = false;
            }
            if (rb != null && isPlayer)
            {
                rb.drag = 0;
                rb.freezeRotation = true;
                rb.useGravity = true;
                isPlayer = false;
            }
            grabbedObject = null;
        }
    }

    /// <summary>
    /// Processes the grab input for this player.
    /// </summary>
    /// <param name="pressing">True if the grab button was pressed.</param>
    public void ProcessGrab(bool pressing)
    {
        // If pressing the grab button, toggle the grabbing state.
        if (pressing)
        {
            if (isGrabbing)
            {
                // If already grabbing, release the object.
                isGrabbing = false;
            }
            else
            {
                // If not grabbing, try to grab an object.
                Ray ray = new Ray(cam.transform.position, cam.transform.forward);
                Debug.DrawRay(ray.origin, ray.direction * 3f, Color.red, 3f);
                if (Physics.Raycast(ray, out RaycastHit hit, 3f))
                {

                    if (hit.transform.gameObject.CompareTag("Grabbable"))
                    {
                        if (!hit.transform.gameObject.GetComponent<RigidVariables>().beingHeld)
                        {
                            grabbedObject = hit.transform.gameObject;
                            isGrabbing = true;
                            isPlayer = false;
                            grabbedObject.GetComponent<RigidVariables>().beingHeld = true;
                        }
                    }
                    if (hit.transform.gameObject.CompareTag("Player"))
                    {
                        grabbedObject = hit.transform.gameObject;
                        isGrabbing = true;
                        isPlayer = true;
                    }
                }
            }

        }


#pragma warning disable CS8321 // Local function is declared but never used
        [MessageHandler((ushort)ClientToServerId.sendGrab)]
        static void Grab(ushort fromClientId, Message message)
        {
            // Retrieve the player object corresponding to the client ID.
            if (Player.list.TryGetValue(fromClientId, out Player player))
            {
                // Retrieve the player's GrabController instance.
                GrabController grabController = player.GetComponentInChildren<GrabController>();
                if (grabController != null)
                {
                    bool pressing = message.GetBool();
                    grabController.ProcessGrab(pressing);
                }
                else
                {
                    Debug.LogError("No GrabController found on player " + fromClientId);
                }
            }
            else
            {
                Debug.LogError("No player found for client " + fromClientId);
            }
        }
#pragma warning restore CS8321 // Local function is declared but never used
    }

    public void Scroll(Vector2 scrollInput)
    {
        Debug.Log(scrollInput);
        if (scrollInput.y > 0)
        {
            float newZPosition = holder.transform.localPosition.z + 0.5f;
            if (newZPosition <= maxGrabRange && newZPosition >= minGrabRange)
            {
                holder.transform.localPosition = new Vector3(holder.transform.localPosition.x, holder.transform.localPosition.y, newZPosition);
            }
        }
        else if (scrollInput.y < 0)
        {
            float newZPosition = holder.transform.localPosition.z - 0.5f;
            if (newZPosition <= maxGrabRange && newZPosition >= minGrabRange)
            {
                holder.transform.localPosition = new Vector3(holder.transform.localPosition.x, holder.transform.localPosition.y, newZPosition);
            }
        }
    }
}


