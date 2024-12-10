using System;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public string selectableTag = "Draggable";

    public delegate void OnObjectSelected(GameObject selectedObject);
    public static event OnObjectSelected ObjectSelectedEvent;
    public static event OnObjectSelected ObjectDetailedEvent;
    public static Action NoObjectHitEventLeft;
    public static Action NoObjectHitEventRight;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void OnEnable()
    {
        InputManager.InputDownEvent += SelectObject;
        InputManager.InputDownRightEvent += DetailObject;
        Debug.Log("SelectionManager: InputManager.InputDownEvent += SelectObject");
    }

    void OnDisable()
    {
        InputManager.InputDownEvent -= SelectObject;
        InputManager.InputDownRightEvent -= DetailObject;
    }

    void SelectObject(Vector3 screenPosition)
    {
        //Debug.Log("SelectObject");

        Ray ray = cam.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        //Debug.DrawRay(cam.transform.position, forward, Color.green);
        Debug.DrawRay(ray.origin, ray.direction * 20, Color.white, 3);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.CompareTag(selectableTag))
            {
                // Emit event, that an object has been selected
                ObjectSelectedEvent?.Invoke(hit.collider.gameObject);
                //Debug.Log("ObjectSelectedEvent");
            }
            else
            {
                NoObjectHitEventLeft?.Invoke();
            }
        }
        else
        {
            NoObjectHitEventLeft?.Invoke();
        }
    }

    void DetailObject(Vector3 screenPosition)
    {
        //Debug.Log("DetailObject");

        Ray ray = cam.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        //Debug.DrawRay(cam.transform.position, forward, Color.green);
        Debug.DrawRay(ray.origin, ray.direction * 20, Color.white, 3);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.CompareTag(selectableTag))
            {
                // Emit event, that an object has been selected
                ObjectDetailedEvent?.Invoke(hit.collider.gameObject);
                //Debug.Log("ObjectDetailedEvent");
            }
            else
            {
                NoObjectHitEventRight?.Invoke();
            }
        }
        else
        {
            NoObjectHitEventRight?.Invoke();
        }
    }
}