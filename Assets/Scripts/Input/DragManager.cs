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

    void StartDragging(GameObject obj)
    {
        selectedObject = obj;
        isDragging = true;

        // Calculate offset from the click point to the object position
        //Vector3 screenPos = cam.WorldToScreenPoint(selectedObject.transform.position);
        //offset = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, screenPos.z)) - selectedObject.transform.position;
        startMousePosition = Input.mousePosition;

        clickPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.WorldToScreenPoint(obj.transform.position).z);
        offset = obj.transform.position - cam.ScreenToWorldPoint(clickPoint);

        if (selectedObject.transform.parent.name == "Hand")
        {
            offset += new Vector3(0, 3, 0);
        }

        Debug.Log($"StartDragging1 {offset} {obj.transform.position}");

        //obj.transform.position = transform.TransformPoint(obj.transform.localPosition);

        Debug.Log($"StartDragging2 {offset} {obj.transform.position}");
    }

    void OnInputMove(Vector3 screenPosition)
    {
        Debug.Log($"OnInputMove {isDragging} {selectedObject != null}");
        if (isDragging && selectedObject != null && startMousePosition != Input.mousePosition)
        {
                
            selectedObject.transform.parent = GameObject.Find("Cards").transform;
            //offset = selectedObject.transform.position - cam.ScreenToWorldPoint(clickPoint);
            //Vector3 worldPos = cam.ScreenToWorldPoint(screenPosition) - offset;
            //selectedObject.transform.position = worldPos;


            Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, cam.WorldToScreenPoint(selectedObject.transform.position).z)) + offset;
            selectedObject.transform.position = worldPos;
        }
    }

    //posprz¹tac 
    //    - offset zalezny czy karta w Hand
    //    - przenoszenie z Hand do Cards na drag
    //    - usunac skalowanie na korutynie - uzywac dotween

    void OnInputUp()
    {
        isDragging = false;
        selectedObject = null;
    }
}