using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    [SerializeField] private int sortingOrder;
    public Vector3 originalScale;
    public Quaternion currentRotation;
    public float albumCardDistance = 2;
    private Vector3 albumPosition ;
    private Quaternion albumRotation;
    private int siblingIndex;
    private int previousSortingOrder = -1;

    private Coroutine scaleCoroutine;
    private Coroutine moveCoroutine;

    [SerializeField] public Dictionary<Vector3, GameObject> objectsInInteraction;

    private void Awake()
    {
        //SelectionManager.ObjectSelectedEvent += ScaleUp();
        //SelectionManager.ObjectSelectedEvent += ScaleUp();

        originalScale = transform.localScale;
        objectsInInteraction = new Dictionary<Vector3, GameObject>();
    }

    public void SetSpriteSortingOrder(int order)
    {
        Debug.Log($"SetSpriteSortingOrder {order}");
        if (previousSortingOrder == -1)
        {
            previousSortingOrder = order;
        }
        else
        {
            previousSortingOrder = GetComponentInChildren<SpriteRenderer>().sortingOrder;
        }
        
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
    public bool IsDetailed()
    {
        return transform.parent.CompareTag("CardDetails");
    }

    public void BringToFront()
    {
        GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(x => x.sortingOrder = 100);
    }

    public void ResetSortingOrder()
    {
        Debug.Log($"ResetSortingOrder {previousSortingOrder}");
        GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(x => x.sortingOrder = previousSortingOrder);
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
                objectsInInteraction.Add(direction, col.gameObject);
            }
        }
    }

    public void ToggleShadersForObjectsInInteraction(bool value)
    {
        foreach (var obj in objectsInInteraction)
        {
            var item = obj.Value;
            var direction = obj.Key;
            if (item.GetComponent<CardBehaviour>() != null)
            {
                if (direction == Vector3.left)
                {
                    SpriteRenderer[] spriteRenderers = item.GetComponentsInChildren<SpriteRenderer>();
                    foreach (var spriteRenderer in spriteRenderers)
                    {
                        var material = spriteRenderer.material;
                        material.SetFloat("_SineGlowFade", value ? 1 : 0);
                    }
                }
                else if (direction == Vector3.right)
                {
                    SpriteRenderer[] spriteRenderers = item.GetComponentsInChildren<SpriteRenderer>();
                    foreach (var spriteRenderer in spriteRenderers)
                    {
                        var material = spriteRenderer.material;
                        material.SetFloat("_SharpenFade", value ? 1 : 0);
                        material.SetFloat("_FrozenFade", value ? 1 : 0);
                    }
                }
                else if (direction == Vector3.up)
                {
                    SpriteRenderer[] spriteRenderers = item.GetComponentsInChildren<SpriteRenderer>();
                    foreach (var spriteRenderer in spriteRenderers)
                    {
                        var material = spriteRenderer.material;
                        material.SetFloat("_EnchantedFade", value ? 1 : 0);
                    }
                }
                else if (direction == Vector3.down)
                {
                    SpriteRenderer[] spriteRenderers = item.GetComponentsInChildren<SpriteRenderer>();
                    foreach (var spriteRenderer in spriteRenderers)
                    {
                        var material = spriteRenderer.material;
                        material.SetFloat("_PoisonFade", value ? 1 : 0);
                    }
                }
            }
        }
    }

    public void TogglePopShaders(bool value)
    {
        SpriteRenderer[] spriteRenderers = this.GetComponentsInChildren<SpriteRenderer>();
        foreach (var spriteRenderer in spriteRenderers)
        {
            var material = spriteRenderer.material;
            material.SetFloat("_SineScaleFrequency", value ? 3 : 0);
            if (spriteRenderer.gameObject.name == "ramka")
            {
                material.SetFloat("_ShineFade", value ? 1 : 0);
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
            material.SetFloat("_PoisonFade", value ? 1 : 0);
            material.SetFloat("_EnchantedFade", value ? 1 : 0);
            material.SetFloat("_SineScaleFrequency", value ? 3 : 0);
        }
    }

    public void CleanObjectsInInteraction()
    {
        ToggleShadersForObjectsInInteraction(false);
        ToggleShaders(false);
        objectsInInteraction.Clear();
    }

    public void EnterDetailState(float scale, Vector3 detailPosition, Transform parent)
    {
        siblingIndex = transform.GetSiblingIndex();
        albumPosition = transform.position;
        albumRotation = transform.rotation;
        SetSpriteSortingOrder(100);

        Debug.Log($"EnterDetailState albumPosition {albumPosition}");
        transform.DORotateQuaternion(Quaternion.identity, 0.2f);
        transform.DOScale(scale, 0.2f);
        transform.DOMove(detailPosition, 0.2f);
        transform.parent = parent;
    }

    public void ExitDetailState(Transform parent)
    {
        Debug.Log($"ExitDetailState albumPosition {albumPosition}.");

        ResetSortingOrder();
        transform.DORotateQuaternion(albumRotation, 0.2f);
        transform.DOMove(albumPosition, 0.2f).onComplete = () => { 
            Debug.Log($"ExitDetailState DOMove complete {albumPosition}."); 
            transform.parent = parent;
            transform.SetSiblingIndex(siblingIndex);
            ;
        };
        transform.DOScale(originalScale, 0.2f);
    }

    public void EnterAlbumAnchoredState(Vector3 anchoredPosition)
    {
        albumPosition = anchoredPosition;

        Debug.Log($"EnterAlbumAnchoredState albumPosition {albumPosition}");

        transform.position = anchoredPosition;
        transform.rotation = Quaternion.identity;
    }
}