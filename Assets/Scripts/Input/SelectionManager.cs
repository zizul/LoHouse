using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public string selectableTag = "Draggable";

    public delegate void OnObjectSelected(GameObject selectedObject);
    public static event OnObjectSelected ObjectSelectedEvent;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void OnEnable()
    {
        InputManager.InputDownEvent += SelectObject;
        Debug.Log("SelectionManager: InputManager.InputDownEvent += SelectObject");
    }

    void OnDisable()
    {
        InputManager.InputDownEvent -= SelectObject;
    }

    void SelectObject(Vector3 screenPosition)
    {
        Debug.Log("SelectObject");

        Ray ray = cam.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.CompareTag(selectableTag))
            {
                // Emit event, that an object has been selected
                ObjectSelectedEvent?.Invoke(hit.collider.gameObject);
                Debug.Log("ObjectSelectedEvent");
            }
            else
            {
                Debug.Log("No hit");
            }
        }
    }
}