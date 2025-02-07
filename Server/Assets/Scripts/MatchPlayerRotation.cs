using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchPlayerRotation : MonoBehaviour
{

    void FixedUpdate()
    {
        transform.eulerAngles = new Vector3(0, transform.parent.eulerAngles.y, 0);
    }

}
