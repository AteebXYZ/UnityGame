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
        initialRotation = transform.rotation;
        targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        timeElapsed = 0f;
        isResetting = true;
    }

    private void Update()
    {
        if (isResetting)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / resetDuration);
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);

            if (t >= 1f)
            {
                isResetting = false;
                transform.rotation = targetRotation; // Ensure final rotation is exact
            }
        }
    }
}
