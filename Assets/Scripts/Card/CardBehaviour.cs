using System.Collections;
using System.Linq;
using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    [SerializeField] private int sortingOrder;
    public Vector3 originalScale;
    public Quaternion currentRotation;

    private Coroutine scaleCoroutine;
    private Coroutine moveCoroutine;

    private void Start()
    {
        //SelectionManager.ObjectSelectedEvent += ScaleUp();
        //SelectionManager.ObjectSelectedEvent += ScaleUp();

        originalScale = transform.localScale;
        sortingOrder = 0;
    }

    public void SetSpriteSortingOrder(int order)
    {
        sortingOrder = order;
        GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(x => x.sortingOrder = order);
    }

    public void SetCardFrontSprite(Sprite sprite)
    {
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }

    public bool IsInHand()
    {
        return transform.parent.CompareTag("Hand");
    }

    public void BringToFront()
    {
        GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(x => x.sortingOrder = 100);
    }

    public void ResetSortingOrder()
    {
        GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(x => x.sortingOrder = sortingOrder);
    }

    public void Scale(Vector3 targetScale, float duration)
    {
        StopScaling();
        scaleCoroutine = StartCoroutine(ScaleObject(targetScale, duration));
    }

    public void ResetScale(float duration)
    {
        StopScaling();
        scaleCoroutine = StartCoroutine(ScaleObject(originalScale, duration));
    }

    public void MovePosition(Vector3 offset, float duration)
    {
        StopMoving();
        scaleCoroutine = StartCoroutine(MoveObject(offset, duration));
    }

    private IEnumerator MoveObject(Vector3 offset, float duration)
    {
        Vector3 currentPosition = transform.localPosition;
        float elapsed = 0f;

        Debug.Log($"Start MoveObject {transform.name} {currentPosition}");

        //while (elapsed < duration)
        //{
        //    transform.localPosition = Vector3.Lerp(currentPosition, currentPosition + offset, elapsed / duration);
        //    //Debug.Log($"MoveObject {transform.localPosition}");
        //    elapsed += Time.deltaTime;
        //    yield return null;
        //}
        yield return null;
        Debug.Log($"End MoveObject1 {transform.name} {transform.localPosition}");
        transform.localPosition = currentPosition + offset;  // Ensure the final scale is exactly the target

        Debug.Log($"End MoveObject2 {transform.name} {transform.localPosition}");
    }

    private IEnumerator ScaleObject(Vector3 targetScale, float duration)
    {
        Vector3 currentScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(currentScale, targetScale, elapsed / duration);
            Debug.Log($"ScaleObject {transform.localScale}");
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;  // Ensure the final scale is exactly the target
    }

    private void StopMoving()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
    }

    private void StopScaling()
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }
    }
}