using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using RiptideNetworking.Utils;

public class GrabController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject holder;
    [SerializeField] private float minGrabRange = 3f;
    [SerializeField] private float maxGrabRange = 6f;

    [Header("Settings")]
    [SerializeField] private float forceMagnitude = 10f;

    private bool isGrabbing = false;
    private GameObject grabbedObject = null;
    private bool isPlayer;
    private Vector2 scrollInput;
    private float rotateInput;
    private Vector2 rotateVector;

    private void Update()
    {
        if (isGrabbing && grabbedObject != null)
        {
            SendGrabbing(true);
            Vector3 direction = holder.transform.position - grabbedObject.transform.position;
            float distance = Vector3.Distance(holder.transform.position, grabbedObject.transform.position);

            if (rotateInput == 1)
            {
                grabbedObject.transform.Rotate(Vector3.up * -rotateVector.x, Space.World);
                grabbedObject.transform.Rotate(Vector3.right, rotateVector.y);
            }

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
                        rb.drag = 1;
                        rb.freezeRotation = true;
                        rb.useGravity = false;
                        rb.AddForce(direction * forceMagnitude, ForceMode.Force);
                        grabbedObject.GetComponent<PlayerMovement>().useRotation = false;
                    }
                }
            }
        }
        else if (grabbedObject != null)
        {
            SendGrabbing(false);
            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
            if (rb != null && grabbedObject.GetComponent<RigidVariables>().beingHeld && !isPlayer)
            {
                rb.drag = 0;
                rb.freezeRotation = false;
                rb.useGravity = true;
                grabbedObject.GetComponent<RigidVariables>().beingHeld = false;
            }
            if (rb != null && isPlayer && grabbedObject.GetComponent<RigidVariables>().beingHeld)
            {
                rb.drag = 0;
                rb.freezeRotation = true;
                rb.useGravity = true;
                isPlayer = false;
                grabbedObject.GetComponent<RigidVariables>().beingHeld = false;
                StartCoroutine(EnableRotationAfterDelay(grabbedObject));
            }
            grabbedObject = null;
        }
    }

    private IEnumerator EnableRotationAfterDelay(GameObject obj)
    {
        yield return new WaitForSeconds(3.5f);
        if (obj != null)
        {
            obj.GetComponent<PlayerMovement>().useRotation = true;
        }
    }

    public void ProcessGrab(bool pressing)
    {
        if (pressing)
        {
            if (isGrabbing)
            {
                isGrabbing = false;
            }
            else
            {
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
                        grabbedObject.GetComponent<RigidVariables>().beingHeld = true;
                    }
                }
            }
        }

#pragma warning disable CS8321
        [MessageHandler((ushort)ClientToServerId.sendGrab)]
        static void Grab(ushort fromClientId, Message message)
        {
            if (Player.list.TryGetValue(fromClientId, out Player player))
            {
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
#pragma warning restore CS8321
    }

    public void Scroll(Vector2 scrollInput)
    {
        Debug.Log(scrollInput);
        float newZPosition = holder.transform.localPosition.z + (scrollInput.y > 0 ? 0.5f : -0.5f);
        if (newZPosition <= maxGrabRange && newZPosition >= minGrabRange)
        {
            holder.transform.localPosition = new Vector3(holder.transform.localPosition.x, holder.transform.localPosition.y, newZPosition);
        }
    }

    public void Rotate(float rotateInput)
    {
        this.rotateInput = rotateInput;
    }
    public void RotateVector(Vector2 rotateVector)
    {
        this.rotateVector = rotateVector;
    }

    private void SendGrabbing(bool grabbing)
    {
        Message message = Message.Create(MessageSendMode.unreliable, ServerToClientId.isGrabbing);
        message.AddBool(grabbing);
        NetworkManager.Singleton.Server.SendToAll(message);
    }
}
