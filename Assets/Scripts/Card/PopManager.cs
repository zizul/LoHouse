using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class PopManager : MonoBehaviour
{
    public Vector3 popScale = new Vector3(1.2f, 1.2f, 1.2f);
    public float popSpeed = 0.2f;
    public Vector3 popMoveOffset = new Vector3(0, 50, 0);

    private Vector3 originalScale;
    private GameObject selectedObject;
    private int popedSortingOrder;

    public static UnityAction<Vector3> ObjectPopedEvent;

    void OnEnable()
    {
        SelectionManager.ObjectSelectedEvent += PopSelectedObject;
        InputManager.InputUpEvent += UnPopSelectedObject;
    }

    void OnDisable()
    {
        SelectionManager.ObjectSelectedEvent -= PopSelectedObject;
        InputManager.InputUpEvent -= UnPopSelectedObject;
    }

    void PopSelectedObject(GameObject obj)
    {
        CardBehaviour card = obj.GetComponent<CardBehaviour>();
        if (card != null && card.IsDetailed())
        {
            // do not pop in details
            return;
        }

        selectedObject = obj;
        originalScale = selectedObject.transform.localScale;

        Vector3 targetScale = new Vector3(
            originalScale.x * popScale.x,
            originalScale.y * popScale.y,
            originalScale.z * popScale.z
        );

        if (card != null && card.IsInHand())
        {
            Debug.Log($"PopSelectedObject {selectedObject.name} {selectedObject.transform.localPosition}");

            card.BringToFront();
            card.transform.DOScale(targetScale, popSpeed);
            //card.transform.DOLocalMove(card.transform.localPosition + popMoveOffset, popSpeed);
            card.transform.DOMove(card.transform.position + popMoveOffset, popSpeed);
            card.transform.DOLocalRotateQuaternion(Quaternion.identity, popSpeed);

            card.CleanObjectsInInteraction();
            card.TogglePopShaders(true);

        }
        else if (card != null && !card.IsInHand())
        {
            card.BringToFront();
            card.transform.DOScale(targetScale, popSpeed);
            card.CleanObjectsInInteraction();
            card.TogglePopShaders(true);
        }
    }

    void UnPopSelectedObject()
    {
        if (selectedObject != null)
        {
            CardBehaviour card = selectedObject.GetComponent<CardBehaviour>();
            if (card != null && card.IsInHand())
            {
                Debug.Log($"UnPopSelectedObject {selectedObject.name} {selectedObject.transform.localPosition}");

                card.ResetSortingOrder();

                card.transform.DOScale(card.originalScale, popSpeed);

                //card.transform.DOLocalMove(card.transform.localPosition - popMoveOffset, popSpeed);

                card.transform.DOMove(card.transform.position - popMoveOffset, popSpeed);
                card.transform.DOLocalRotateQuaternion(card.currentRotation, popSpeed);
                //card.ResetScale(popSpeed);
                //card.MovePosition(-popMoveOffset, popSpeed);
            }
            else if (card != null && !card.IsInHand())
            {
                card.ResetSortingOrder();

                card.transform.DOScale(card.originalScale, popSpeed);
            }

            selectedObject = null;
        }
    }


}