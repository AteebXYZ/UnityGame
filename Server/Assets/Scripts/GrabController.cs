using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using RiptideNetworking.Utils;

public class GrabController : MonoBehaviour
{
    // Assign this in the Inspector
    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject holder;
    [SerializeField] private float forceMagnitude;



    // Singleton instance so that the static method can reference it
    public static GrabController Instance { get; private set; }

    private void Awake()
    {
        // Set the instance (you might want to add checks for multiple instances)
        Instance = this;
    }


    private static bool down = false;
    private static GameObject tempObject;
    private void Update()
    {
        if (down)
        {
            Vector3 direction = (holder.transform.position - tempObject.transform.position).normalized;
            float distance = Vector3.Distance(holder.transform.position, tempObject.transform.position);
            if (distance > 0.4f)
            {
                Rigidbody rb = tempObject.GetComponent<Rigidbody>();
                rb.drag = 10;
                rb.freezeRotation = true;
                rb.useGravity = false;
                rb.AddForce(direction * forceMagnitude, ForceMode.Force);
            }

        }
        else
        {
            if (tempObject != null)
            {

                Rigidbody rb = tempObject.GetComponent<Rigidbody>();

                rb.drag = 0;
                rb.freezeRotation = false;
                rb.useGravity = true;
                tempObject = null;
            }
        }
    }
    [MessageHandler((ushort)ClientToServerId.sendGrab)]
    private static void Grab(ushort fromClientId, Message message)
    {

        bool pressing = message.GetBool();
        if (Instance == null)
        {
            Debug.LogError("GrabController instance is not set!");
            return;
        }



        if (pressing)
        {
            if (down == true)
            {
                down = false;

            }
            else
            {
                Ray ray = new Ray(Instance.cam.transform.position, Instance.cam.transform.forward);
                Debug.DrawRay(ray.origin, ray.direction * 3f, Color.red, 3f);
                if (Physics.Raycast(ray, out RaycastHit hit, 3f))
                {
                    Debug.Log(hit.transform.gameObject.name);
                    if (hit.transform.gameObject.CompareTag("Grabbable"))
                    {
                        tempObject = hit.transform.gameObject;
                        down = true;
                    }
                }

            }
        }

        Debug.Log(down);
    }
}


