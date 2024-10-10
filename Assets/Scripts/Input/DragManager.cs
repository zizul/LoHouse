using UnityEngine;

public class DragHandler : MonoBehaviour
{
    private Camera cam;
    private GameObject selectedObject;
    private Vector3 offset;
    private bool isDragging = false;

    void OnEnable()
    {
        SelectionManager.ObjectSelectedEvent += OnObjectSelected;
        InputManager.InputMoveEvent += OnInputMove;
        InputManager.InputUpEvent += OnInputUp;
    }

    void OnDisable()
    {
        SelectionManager.ObjectSelectedEvent -= OnObjectSelected;
        InputManager.InputMoveEvent -= OnInputMove;
        InputManager.InputUpEvent -= OnInputUp;
    }

    void Start()
    {
        cam = Camera.main;
    }

    void OnObjectSelected(GameObject obj)
    {
        selectedObject = obj;
        isDragging = true;

        // Calculate offset from the click point to the object position
        Vector3 screenPos = cam.WorldToScreenPoint(selectedObject.transform.position);
        offset = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, screenPos.z)) - selectedObject.transform.position;
    }

    void OnInputMove(Vector3 screenPosition)
    {
        if (isDragging && selectedObject != null)
        {
            Vector3 worldPos = cam.ScreenToWorldPoint(screenPosition) - offset;
            selectedObject.transform.position = worldPos;
        }
    }

    void OnInputUp()
    {
        isDragging = false;
        selectedObject = null;
    }
}