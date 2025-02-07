using UnityEngine;

public class PlayerReset : MonoBehaviour
{
    private float resetDuration = 0.5f; // Adjust duration as needed
    private float timeElapsed = 0f;
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private bool isResetting = false;

    private void Start()
    {
        // Optional: Initialize anything if needed
    }

    public void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);
    }

    private void Update()
    {
    }
}
