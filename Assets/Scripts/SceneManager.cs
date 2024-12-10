using DG.Tweening;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public GameObject detailsPanel;
    public GameObject detailsCardHolder;
    public GameObject detailsEffects;
    public Transform previousContainer;
    private GameObject detailedCard;
    
    public float detailedCardScale;

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
        Debug.Log("SelectionManager: InputManager.InputDownRightEvent += ChangeScene");
    }

    void OnDisable()
    {
        SelectionManager.ObjectDetailedEvent -= ShowCardDetailsView;
        SelectionManager.NoObjectHitEventLeft += HideCardDetailsView;
        SelectionManager.NoObjectHitEventRight += HideCardDetailsView;
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
                //cardBehaviour.transform.parent = detailsCardHolder.transform;
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
                //cardBehaviour.transform.parent = previousContainer;
            }
            detailedCard = null;
            previousContainer = null;
        }
    }
}