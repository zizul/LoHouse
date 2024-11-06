using UnityEngine;

public class InputManager : MonoBehaviour
{
    public delegate void OnInputDown(Vector3 position);
    public delegate void OnInputUp();
    public delegate void OnInputMove(Vector3 position);

    public static event OnInputDown InputDownEvent;
    public static event OnInputUp InputUpEvent;
    public static event OnInputMove InputMoveEvent;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.touchSupported)
        {
            HandleTouchInput();
        }
        else
        {
            HandleMouseInput();
        }
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("InputDownEvent");
            Vector3 pos = Input.mousePosition;
            //pos.z = cam.WorldToScreenPoint(Vector3.zero).z;
            InputDownEvent?.Invoke(pos);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 pos = Input.mousePosition;
            //pos.z = cam.WorldToScreenPoint(Vector3.zero).z;
            InputMoveEvent?.Invoke(pos);
            //Debug.Log("InputMoveEvent");
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("InputUpEvent");
            InputUpEvent?.Invoke();
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 pos = touch.position;
            //pos.z = cam.WorldToScreenPoint(Vector3.zero).z;

            if (touch.phase == TouchPhase.Began)
            {
                InputDownEvent?.Invoke(pos);
            }

            if (touch.phase == TouchPhase.Moved)
            {
                InputMoveEvent?.Invoke(pos);
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                InputUpEvent?.Invoke();
            }
        }
    }
}