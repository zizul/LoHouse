using UnityEditor.AnimatedValues;
using UnityEngine;

public class ObjectRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Speed of rotation in degrees per second")]
    public float rotationSpeed = 90f;

    [Tooltip("Whether rotation should be clockwise or counter-clockwise")]
    public bool clockwise = true;

    [Tooltip("Whether to start rotating automatically")]
    public bool rotateOnStart = true;

    [Tooltip("Use this to manually enable/disable rotation during runtime")]
    public bool isRotating;

    public Vector3 axis = Vector3.up;

    private Vector3 centerPoint;
    private Bounds objectBounds;

    private void Start()
    {
        // Initialize rotation state based on inspector setting
        isRotating = rotateOnStart;

        // Calculate the true center of the object using its renderer bounds
        //CalculateObjectCenter();
    }

    public void CalculateObjectCenter()
    {
        // Get all renderers (in case object has multiple parts)
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        // Calculate bounds that encompass all renderers
        objectBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            objectBounds.Encapsulate(renderers[i].bounds);
        }

        // Store the center point
        centerPoint = objectBounds.center;
    }

    private void Update()
    {
        //if (isRotating)
        //{
        //    // Calculate rotation direction
        //    float direction = clockwise ? -1f : 1f;
        //    float angle = rotationSpeed * direction * Time.deltaTime;

        //    // Rotate around the center point
        //    transform.RotateAround(centerPoint, Vector3.up, angle);
        //}

        if (isRotating)
        {
            // Calculate rotation direction
            float direction = clockwise ? -1f : 1f;

            // Apply rotation around local Y axis (this will rotate around object's center)
            //transform.Rotate(0f, rotationSpeed * direction * Time.deltaTime, 0f, Space.Self);
            transform.Rotate(axis * (rotationSpeed * direction * Time.deltaTime));
        }
    }

    // Public methods to control rotation
    public void StartRotation()
    {
        isRotating = true;
    }

    public void StopRotation()
    {
        isRotating = false;
    }

    public void ToggleRotation()
    {
        isRotating = !isRotating;
    }

    public void SetRotationSpeed(float newSpeed)
    {
        rotationSpeed = newSpeed;
    }

    public void SetRotationDirection(bool newClockwise)
    {
        clockwise = newClockwise;
    }
}