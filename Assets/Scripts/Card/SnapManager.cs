using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SnapManager : MonoBehaviour
{
    public List<GameObject> anchorPoints = new List<GameObject>();
    private GameObject selectedObject;
    public float snapDistance = 1.0f;

    void OnEnable()
    {
        SelectionManager.ObjectSelectedEvent += OnObjectSelected;
        InputManager.InputUpEvent += OnInputUp;
        anchorPoints = GameObject.FindGameObjectsWithTag("CardAnchor").ToList();
    }

    void OnDisable()
    {
        SelectionManager.ObjectSelectedEvent -= OnObjectSelected;
        InputManager.InputUpEvent -= OnInputUp;
    }

    void OnObjectSelected(GameObject obj)
    {
        selectedObject = obj;
    }

    void OnInputUp()
    {
        if (selectedObject != null)
        {
            CardBehaviour card = selectedObject.GetComponent<CardBehaviour>();
            if (card != null && !card.IsInHand())
            {
                SnapToClosestAnchor(selectedObject);
            }
            selectedObject = null;
        }
    }

    private void SnapToClosestAnchor(GameObject obj)
    {
        float closestDistance = Mathf.Infinity;
        GameObject closestAnchor = null;

        foreach (GameObject anchor in anchorPoints)
        {
            float distance = Vector3.Distance(obj.transform.position, anchor.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestAnchor = anchor;
            }
        }

        Debug.Log($"closestDistance: {closestDistance}");
        Debug.Log($"closestAnchor: {closestAnchor}");

        if (closestAnchor != null && closestDistance <= snapDistance) // Mo¿esz dostosowaæ próg bliskoœci
        {
            CardBehaviour cardBehaviour = obj.GetComponent<CardBehaviour>();
            if (cardBehaviour != null)
            {
                cardBehaviour.EnterAlbumAnchoredState(closestAnchor.transform.position);
                cardBehaviour.IsOtherCardNearby(Vector3.left);
                cardBehaviour.IsOtherCardNearby(Vector3.right);
                cardBehaviour.IsOtherCardNearby(Vector3.up);
                cardBehaviour.IsOtherCardNearby(Vector3.down);
                cardBehaviour.ToggleShaders(false);
                cardBehaviour.ToggleShadersForObjectsInInteraction(true);
            }
        }
    }
}
