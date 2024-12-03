using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    [SerializeField] private int sortingOrder;
    public Vector3 originalScale;
    public Quaternion currentRotation;
    public float albumCardDistance = 2;

    private Coroutine scaleCoroutine;
    private Coroutine moveCoroutine;

    [SerializeField] public List<GameObject> objectsInInteraction;

    private void Start()
    {
        //SelectionManager.ObjectSelectedEvent += ScaleUp();
        //SelectionManager.ObjectSelectedEvent += ScaleUp();

        originalScale = transform.localScale;
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

    private void OnDrawGizmos()
    {
        // Tylko jeœli mamy obiekt do z³apania
        Vector3 targetPosition = transform.position + Vector3.left * albumCardDistance;

        // Narysuj kulkê w targetPosition
        Gizmos.color = Color.red; // Gangsterska czerwieñ, wiadomo.
        Gizmos.DrawSphere(targetPosition, 0.2f);

    }

    public void IsOtherCardNearby(Vector3 direction)
    {
        // SprawdŸ odleg³oœæ dla kotwicy po lewej lub prawej stronie
        Vector3 targetPosition = transform.position + direction * albumCardDistance;

        var colliders = Physics.OverlapSphere(targetPosition, 0.2f);
        foreach (var col in colliders)
        {
            if (!ReferenceEquals(col.gameObject, this.gameObject) && col.GetComponent<CardBehaviour>() != null)
            {
                objectsInInteraction.Add(col.gameObject);
            }
        }
    }

    public void ToggleShadersForObjectsInInteraction(bool value)
    {
        foreach (var item in objectsInInteraction)
        {
            if (item.GetComponent<CardBehaviour>() != null)
            {
                if (item.transform.position.x < transform.position.x)
                {
                    SpriteRenderer[] spriteRenderers = item.GetComponentsInChildren<SpriteRenderer>();
                    foreach (var spriteRenderer in spriteRenderers)
                    {
                        var material = spriteRenderer.material;
                        material.SetFloat("_SineGlowFade", value ? 1 : 0);
                    }
                }
                else
                {
                    SpriteRenderer[] spriteRenderers = item.GetComponentsInChildren<SpriteRenderer>();
                    foreach (var spriteRenderer in spriteRenderers)
                    {
                        var material = spriteRenderer.material;
                        material.SetFloat("_SharpenFade", value ? 1 : 0);
                        material.SetFloat("_FrozenFade", value ? 1 : 0);
                    }
                }
            }
        }
    }

    public void ToggleShaders(bool value)
    {
        SpriteRenderer[] spriteRenderers = this.GetComponentsInChildren<SpriteRenderer>();
        foreach (var spriteRenderer in spriteRenderers)
        {
            var material = spriteRenderer.material;
            material.SetFloat("_ShineFade", value ? 1 : 0);
            material.SetFloat("_FrozenFade", value ? 1 : 0);
            material.SetFloat("_SineGlowFade", value ? 1 : 0);
            material.SetFloat("_SharpenFade", value ? 1 : 0);
        }
    }

    public void CleanObjectsInInteraction()
    {
        ToggleShadersForObjectsInInteraction(false);
        ToggleShaders(false);
        objectsInInteraction.Clear();
    }
}