using DG.Tweening;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public GameObject detailsPanel;
    public GameObject detailsCardHolder;
    public GameObject detailsEffects;
    private Transform previousContainer;
    private GameObject detailedCard;
    public float detailedCardScale;

    public GameObject dialogPanel;
    public GameObject dialogCardHolder;
    private GameObject dialogCard;

    //public delegate void OnObjectSelected(GameObject selectedObject);
    //public static event OnObjectSelected ObjectSelectedEvent;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void OnEnable()
    {

        //InputManager.InputDownEvent += HideCardDetailsView;
        SelectionManager.NoObjectHitEventLeft += HideCardDetailsView;
        SelectionManager.NoObjectHitEventRight += HideCardDetailsView;
        SelectionManager.ObjectDetailedEvent += ShowCardDetailsView;


        SelectionManager.NoObjectHitEventLeft += HideCardDialogView;
        SelectionManager.NoObjectHitEventRight += HideCardDetailsView;

        SelectionManager.ObjectSelectedEvent += HideCardDialogView;

        Debug.Log("SelectionManager: InputManager.InputDownRightEvent += ChangeScene");
    }

    void OnDisable()
    {
        SelectionManager.ObjectDetailedEvent -= ShowCardDetailsView;
        SelectionManager.NoObjectHitEventLeft += HideCardDetailsView;
        SelectionManager.NoObjectHitEventRight += HideCardDetailsView;

        SelectionManager.NoObjectHitEventLeft += HideCardDetailsView;
        SelectionManager.NoObjectHitEventRight += HideCardDialogView;

        SelectionManager.ObjectSelectedEvent -= HideCardDialogView;
    }

    void ShowCardDetailsView(GameObject cardGO)
    {
        if (detailsPanel.activeSelf == false)
        {
            detailedCard = cardGO;
            previousContainer = detailedCard.transform.parent;
            Debug.Log("ChangeScene");
            detailsEffects.SetActive(true);
            detailsPanel.SetActive(true);

            CardBehaviour cardBehaviour = cardGO.GetComponent<CardBehaviour>();
            if (cardBehaviour != null)
            {
                cardBehaviour.EnterDetailState(detailedCardScale, detailsCardHolder.transform.position, detailsCardHolder.transform);
            }
        }
    }

    void HideCardDetailsView()
    {
        if (detailsPanel.activeSelf == true && detailedCard != null)
        {
            detailsEffects.SetActive(false);
            detailsPanel.SetActive(false);

            CardBehaviour cardBehaviour = detailedCard.GetComponent<CardBehaviour>();
            if (cardBehaviour != null)
            {
                cardBehaviour.ExitDetailState(previousContainer);
            }
            detailedCard = null;
            previousContainer = null;
        }
    }

    public void ShowCardDialogView(GameObject cardGO)
    {
        if (dialogPanel.activeSelf == false)
        {
            dialogCard = Instantiate(cardGO, dialogCardHolder.transform);

            //CardBehaviour cardBehaviour = dialogCard.GetComponent<CardBehaviour>();
            //if (cardBehaviour != null)
            //{
            //    cardBehaviour.EnterDialogState();
            //}

            CardBehaviour dialogCardBehaviour = dialogCard.GetComponent<CardBehaviour>();
            if (dialogCardBehaviour != null)
            {
                dialogCardBehaviour.EnterDialogState();
            }

            SpriteRenderer[] spriteRenderers = dialogCard.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.material = new Material(spriteRenderer.material); // Kopiujemy materia³
            }

            dialogPanel.SetActive(true);
            dialogPanel.transform.DOLocalMoveY(-366, 0.5f).SetEase(Ease.InBack)/*.onComplete = () => Instantiate(cardGO, dialogCardHolder.transform)*/;
        }
    }

    public void HideCardDialogView()
    {
        if (dialogPanel.activeSelf == true)
        {
            //GameObject dialogDuplicateCard = Instantiate(cardGO, dialogCardHolder.transform);

            dialogPanel.transform.DOLocalMoveY(-681, 0.5f).SetEase(Ease.InBack).onComplete = () =>
            {
                dialogPanel.SetActive(false);
                Destroy(dialogCard);
                dialogCard = null;
            };
        }
    }

    public void HideCardDialogView(GameObject cardGO)
    {
        HideCardDialogView();
    }
}