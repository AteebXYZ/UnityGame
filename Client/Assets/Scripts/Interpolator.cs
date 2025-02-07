using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpolator : MonoBehaviour
{
    [SerializeField] private float timeElapsed = 0f;
    [SerializeField] private float timeToReachTarget = 0.05f;
    [SerializeField] private float movementThreshold = 0.05f;

    private readonly List<TransformUpdate> futureTransformUpdates = new List<TransformUpdate>();
    private float squareMovementThreshold;
    private TransformUpdate to;
    private TransformUpdate from;
    private TransformUpdate previous;
    private float squareRotationThreshold = 0f;

    private void Start()
    {
        squareMovementThreshold = movementThreshold * movementThreshold;
        to = new TransformUpdate(NetworkManager.Singleton.ServerTick, transform.position, transform.rotation);
        from = new TransformUpdate(NetworkManager.Singleton.InterpolationTick, transform.position, transform.rotation);
        previous = new TransformUpdate(NetworkManager.Singleton.InterpolationTick, transform.position, transform.rotation);
    }

    private void Update()
    {
        for (int i = 0; i < futureTransformUpdates.Count; i++)
        {
            if (NetworkManager.Singleton.ServerTick >= futureTransformUpdates[i].Tick)
            {
                previous = to;
                to = futureTransformUpdates[i];
                from = new TransformUpdate(NetworkManager.Singleton.InterpolationTick, transform.position, transform.rotation);
                futureTransformUpdates.RemoveAt(i);
                i--;
                timeElapsed = 0f;

                float ticksToReach = Mathf.Max(1, from.Tick - to.Tick);
                if (from.Tick - to.Tick != 0)
                {
                    // Debug.Log(from.Tick - to.Tick);
                }
                timeToReachTarget = ticksToReach * Time.fixedDeltaTime;

            }
        }

        timeElapsed += Time.deltaTime;
        // if (timeElapsed / timeToReachTarget > 1.5) { Debug.Log(timeElapsed / timeToReachTarget); }

        InterpolatePosition(timeElapsed / timeToReachTarget);
    }

    private void InterpolatePosition(float lerpAmount)
    {
        bool shouldInterpolatePosition = (to.Position - previous.Position).sqrMagnitude >= squareMovementThreshold || to.Position != from.Position;
        bool shouldInterpolateRotation = Quaternion.Angle(to.Rotation, previous.Rotation) >= squareRotationThreshold || to.Rotation != from.Rotation;

        if (shouldInterpolatePosition)
        {
            transform.position = Vector3.LerpUnclamped(from.Position, to.Position, lerpAmount);
        }
        else
        {
            transform.position = Vector3.Lerp(previous.Position, to.Position, lerpAmount);
        }

        if (shouldInterpolateRotation)
        {
            // transform.rotation = Quaternion.SlerpUnclamped(from.Rotation, to.Rotation, lerpAmount);
            transform.rotation = to.Rotation;
        }
        else
        {
            // transform.rotation = Quaternion.Slerp(previous.Rotation, to.Rotation, lerpAmount);
            transform.rotation = to.Rotation;
        }
    }

    public void NewUpdate(int tick, Vector3 position, Quaternion rotation)
    {
        if (tick <= NetworkManager.Singleton.InterpolationTick)
        {
            return;
        }

        for (int i = 0; i < futureTransformUpdates.Count; i++)
        {
            if (tick < futureTransformUpdates[i].Tick)
            {
                futureTransformUpdates.Insert(i, new TransformUpdate(tick, position, rotation));
                return;
            }
        }

        futureTransformUpdates.Add(new TransformUpdate(tick, position, rotation));
    }

}
