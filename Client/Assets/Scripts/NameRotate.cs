using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameRotate : MonoBehaviour
{
    private Transform trans;
    private void Start()
    {
        trans = GameObject.Find("Camera").GetComponent<Transform>();
    }
    private void Update()
    {
        Debug.Log(Camera.main);
        transform.LookAt(trans);
        transform.Rotate(0, 180, 0);

    }
}
