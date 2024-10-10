using UnityEngine;
using System.Collections;

public class PopManager : MonoBehaviour
{
    public Vector3 popScale = new Vector3(1.2f, 1.2f, 1.2f);
    public float popSpeed = 0.2f;

    private Vector3 originalScale;
    private GameObject selectedObject;
    private Coroutine scaleCoroutine;

    void OnEnable()
    {
        SelectionManager.ObjectSelectedEvent += OnObjectSelected;
        InputManager.InputUpEvent += OnInputUp;
    }

    void OnDisable()
    {
        SelectionManager.ObjectSelectedEvent -= OnObjectSelected;
        InputManager.InputUpEvent -= OnInputUp;
    }

    void OnObjectSelected(GameObject obj)
    {
        if (selectedObject != null)
        {
            StopScaling();  // Stop any previous scaling
        }

        selectedObject = obj;
        originalScale = selectedObject.transform.localScale;

        Vector3 targetScale = new Vector3(
            originalScale.x * popScale.x,
            originalScale.y * popScale.y,
            originalScale.z * popScale.z
        );

        // Start scaling up (pop effect)
        scaleCoroutine = StartCoroutine(ScaleObject(selectedObject, targetScale, popSpeed));
    }

    void OnInputUp()
    {
        if (selectedObject != null)
        {
            // Start scaling back to the original scale
            if (scaleCoroutine != null)
            {
                StopCoroutine(scaleCoroutine);
            }
            scaleCoroutine = StartCoroutine(ScaleObject(selectedObject, originalScale, popSpeed));
            selectedObject = null;
        }
    }

    private IEnumerator ScaleObject(GameObject obj, Vector3 targetScale, float duration)
    {
        Vector3 currentScale = obj.transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            obj.transform.localScale = Vector3.Lerp(currentScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = targetScale;  // Ensure the final scale is exactly the target
    }

    private void StopScaling()
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }
    }
}