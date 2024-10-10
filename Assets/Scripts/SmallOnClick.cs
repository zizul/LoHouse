using UnityEngine;
using UnityEngine.EventSystems;

//[RequireComponent(typeof(Collider))]

public class SmallOnClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Vector3 normalScale;
    public Vector3 smallScale;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = smallScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        transform.localScale = normalScale;
    }
}