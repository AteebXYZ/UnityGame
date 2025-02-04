using System.Collections.Generic;
using UnityEngine;

public class RigidbodyInterpolator : MonoBehaviour
{
    [SerializeField] private float timeElapsed = 0f;
    [SerializeField] private float timeToReachTarget = 0.05f;
    [SerializeField] private float movementThreshold = 0.05f;

    private readonly List<TransformUpdate> futureTransformUpdates = new List<TransformUpdate>();
    private float squareMovementThreshold;
    private TransformUpdate to;
    private TransformUpdate from;
    private TransformUpdate previous;
    private bool hasReceivedFirstUpdate = false; // Prevents falling through floor

    private void Start()
    {
        squareMovementThreshold = movementThreshold * movementThreshold;
        to = new TransformUpdate(NetworkManager.Singleton.ServerTick, transform.position, transform.rotation);
        from = new TransformUpdate(NetworkManager.Singleton.InterpolationTick, transform.position, transform.rotation);
        previous = new TransformUpdate(NetworkManager.Singleton.InterpolationTick, transform.position, transform.rotation);
    }

    private void Update()
    {
        // if (!hasReceivedFirstUpdate)
        // {
        //     // Ensure we don't move objects until we receive at least one valid update
        //     return;
        // }

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
                timeToReachTarget = ticksToReach * Time.fixedDeltaTime;
            }
        }

        timeElapsed += Time.deltaTime;
        InterpolateTransform(timeElapsed / timeToReachTarget);
    }

    private void InterpolateTransform(float lerpAmount)
    {
        if (!hasReceivedFirstUpdate) return;

        if ((to.Position - previous.Position).sqrMagnitude < squareMovementThreshold)
        {
            if (to.Position != from.Position)
            {
                transform.position = Vector3.Lerp(previous.Position, to.Position, lerpAmount);
                transform.rotation = Quaternion.Slerp(previous.Rotation, to.Rotation, lerpAmount);
            }
            return;
        }

        transform.position = Vector3.LerpUnclamped(from.Position, to.Position, lerpAmount);
        transform.rotation = Quaternion.SlerpUnclamped(from.Rotation, to.Rotation, lerpAmount);
    }

    public void NewUpdate(int tick, Vector3 position, Quaternion rotation)
    {
        if (tick <= NetworkManager.Singleton.InterpolationTick)
        {
            return;
        }

        hasReceivedFirstUpdate = true; // Ensure we start interpolation

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

    private struct TransformUpdate
    {
        public int Tick { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public TransformUpdate(int tick, Vector3 position, Quaternion rotation)
        {
            Tick = tick;
            Position = position;
            Rotation = rotation;
        }
    }
}
