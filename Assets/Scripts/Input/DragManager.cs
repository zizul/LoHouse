using UnityEngine;

public class DragHandler : MonoBehaviour
{
    private Camera cam;
    private GameObject selectedObject;
    public Vector3 offset;
    private bool isDragging = false;
    private Vector3 startMousePosition;
    private Vector3 clickPoint;

    void OnEnable()
    {
        SelectionManager.ObjectSelectedEvent += StartDragging;
        InputManager.InputMoveEvent += OnInputMove;
        InputManager.InputUpEvent += OnInputUp;
    }

    void OnDisable()
    {
        SelectionManager.ObjectSelectedEvent -= StartDragging;
        InputManager.InputMoveEvent -= OnInputMove;
        InputManager.InputUpEvent -= OnInputUp;
    }

    void Start()
    {
        cam = Camera.main;
    }

    public void RecalculateOffset(Vector3 position)
    {
        offset = position - cam.ScreenToWorldPoint(clickPoint);
    }

    [Header("Tilt Settings")]
    [Tooltip("Maximum tilt angle during drag")]
    public float maxTiltAngle = 30f;

    [Tooltip("Multiplier for converting drag speed to tilt angle")]
    public float tiltSpeedMultiplier = 0.2f;

    [Tooltip("Smoothing speed for tilt rotation")]
    public float tiltSmoothSpeed = 10f;

    [Tooltip("Minimum drag speed required to start tilting")]
    public float minDragSpeedForTilt = 0.1f;

    Vector3 lastMousePosition;

    void OnInputMove(Vector3 screenPosition)
    {
        //Debug.Log($"OnInputMove {isDragging} {selectedObject != null}");
        if (isDragging && selectedObject != null && startMousePosition != Input.mousePosition)
        {
            // Calculate drag velocity
            Vector2 dragDelta = (Vector2)Input.mousePosition - (Vector2)lastMousePosition;
            float dragSpeed = dragDelta.magnitude;

            // Move the card
            selectedObject.transform.parent = GameObject.Find("Cards").transform;
            Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, cam.WorldToScreenPoint(selectedObject.transform.position).z)) + offset;
            selectedObject.transform.position = worldPos;

            // Tilt logic with parameterized settings
            if (dragSpeed > minDragSpeedForTilt)
            {
                // Calculate tilt axis and angle
                Vector3 tiltAxis = Vector3.Cross(dragDelta, Vector3.forward).normalized;

                // Calculate tilt angle based on drag speed and parameters
                float tiltAngle = Mathf.Clamp(
                    dragSpeed * tiltSpeedMultiplier,
                    0f,
                    maxTiltAngle
                );

                // Create tilt rotation
                Quaternion tiltRotation = Quaternion.AngleAxis(tiltAngle, tiltAxis);

                // Apply smooth interpolation to tilt
                selectedObject.transform.rotation = Quaternion.Slerp(
                    selectedObject.transform.rotation,
                    tiltRotation,
                    Time.deltaTime * tiltSmoothSpeed
                );
            }
            else
            {
                // Optional: Smoothly return to original rotation when drag speed is low
                selectedObject.transform.rotation = Quaternion.Slerp(
                    selectedObject.transform.rotation,
                    Quaternion.identity,
                    Time.deltaTime * tiltSmoothSpeed
                );
            }

            // Update last mouse position for next frame's velocity calculation
            lastMousePosition = Input.mousePosition;
        }
    }

    void StartDragging(GameObject obj)
    {
        selectedObject = obj;
        isDragging = true;
        startMousePosition = Input.mousePosition;
        lastMousePosition = startMousePosition;

        clickPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.WorldToScreenPoint(obj.transform.position).z);
        offset = obj.transform.position - cam.ScreenToWorldPoint(clickPoint);

        if (selectedObject.transform.parent.name == "Hand")
        {
            offset += new Vector3(0, 3, 0);
        }

        Debug.Log($"StartDragging1 {offset} {obj.transform.position}");
    }

    void OnInputUp()
    {
        isDragging = false;
        selectedObject = null;
    }
}