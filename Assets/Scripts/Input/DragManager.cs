using UnityEngine;

public class DragHandler : MonoBehaviour
{
    private Camera cam;
    private GameObject selectedObject;
    private Vector3 offset;
    private bool isDragging = false;

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

    void StartDragging(GameObject obj)
    {
        selectedObject = obj;
        isDragging = true;

        // Calculate offset from the click point to the object position
        //Vector3 screenPos = cam.WorldToScreenPoint(selectedObject.transform.position);
        //offset = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, screenPos.z)) - selectedObject.transform.position;

        Vector3 clickPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.WorldToScreenPoint(obj.transform.position).z);
        offset = obj.transform.position - cam.ScreenToWorldPoint(clickPoint);
    }

    void OnInputMove(Vector3 screenPosition)
    {
        if (isDragging && selectedObject != null)
        {
            //Vector3 worldPos = cam.ScreenToWorldPoint(screenPosition) - offset;
            //selectedObject.transform.position = worldPos;

            Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, cam.WorldToScreenPoint(selectedObject.transform.position).z)) + offset;
            selectedObject.transform.position = worldPos;
        }
    }

    void OnInputUp()
    {
        isDragging = false;
        selectedObject = null;
    }
}