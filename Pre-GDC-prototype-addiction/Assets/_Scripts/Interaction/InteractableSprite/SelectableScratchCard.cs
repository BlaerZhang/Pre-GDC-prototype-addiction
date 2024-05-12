using System;
using _Scripts.FaceEventSystem;
using _Scripts.Manager;
using _Scripts.MetaphysicsSystem;
using _Scripts.ScratchCardGeneration.Utilities;
using _Scripts.VisualTools;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.Interaction.InteractableSprite
{
    public class SelectableScratchCard : MonoBehaviour
    {
        // [Header("Drag")]
        // public float dragSpeed = 1;
        // private bool isDragging = false;
        // private bool isInBuyArea = false;
    
        [Header("Feedback")] 
        public float hoverScale = 0.95f;
        
        [Header("Metaphysics")]
        public FaceType faceType;

        [HideInInspector] public SpriteRenderer cardFaceSprite;
        [HideInInspector] public SpriteRenderer cardBGSprite;
        [HideInInspector] public SpriteShadow[] shadows;
        // private Vector2 dragOffset = new Vector2(0, 0);

        // private BuyCardManager buyCardManager;

        public static Action<SelectableScratchCard, FaceEventType> onScratchCardSelected;

        private bool isCardSelected = false;

        private void OnEnable()
        {
            onScratchCardSelected += ChangeSelectableState;
        }

        private void OnDisable()
        {
            onScratchCardSelected -= ChangeSelectableState;
        }

        void Start()
        {
            cardFaceSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
            cardBGSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
            shadows = GetComponentsInChildren<SpriteShadow>();
            // buyCardManager = FindObjectOfType<BuyCardManager>();
            // isDragging = false;
        }

        private void PickCard()
        {
            FaceEventType faceEventTypeResult = Utils.CalculateMultiProbability(GameManager.Instance.cardPoolManager.eventTriggerWeightPerFaceTypeDict[faceType]); //draw face event
            StatsTracker.onValueChanged?.Invoke(nameof(faceEventTypeResult), (int)faceEventTypeResult); //send to metaphysics center

            //start collect + zoom
            // StartCoroutine(BuyCardCoroutineChain(faceEventTypeResult));

            onScratchCardSelected?.Invoke(this, faceEventTypeResult);
        }

        private void ChangeSelectableState(SelectableScratchCard card, FaceEventType type)
        {
            isCardSelected = true;
        }

        // IEnumerator BuyCardCoroutineChain(FaceEventType faceEventTypeResult)
        // {
            // if (faceEventTypeResult != FaceEventType.NoEvent) yield return new WaitForSeconds(1);
            // yield return StartCoroutine(CollectCards(isPurchased));
            // ZoomInCard();
        // }

        // private void ZoomInCard()
        // {
        //     //set initial position
        //     transform.DOMove(Vector2.zero, 0.2f);
        //     transform.rotation = Quaternion.Euler(Vector3.zero);
        //
        //     //set order
        //     // card.cardFaceSprite.sortingOrder = 6;
        // }

        protected void OnMouseEnter()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            // if (!isDragging)
            // {
                // base.OnMouseEnter();
            foreach (var shadow in shadows)
            {
                SpriteRenderer shadowSprite = shadow.transform.Find("Shadow").GetComponent<SpriteRenderer>();
                shadowSprite.material.DOColor(shadow.hoverShadowColor, 0.1f);
            }
            // }
        }
        
        protected void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            // isDragging = true;
            
            // base.OnMouseDown();
        
            //Reorder in List and Sort order
            // buyCardManager.cardsToBuy.Remove(this);
            // buyCardManager.cardsToBuy.Insert(0,this);
            // buyCardManager.SortCardsOrder();
            // cardSprite.sortingOrder = 10;
        
            //Scale
            cardFaceSprite.transform.DOScale(Vector3.one * hoverScale, 0.1f);
        
            //Set Drag Point Offset
            // dragOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
            //Activate Buy Area
            // buyCardManager.ActivateBuyArea();
            
            //Deactivate ScratchOff Button
            // buyCardManager.DeactivateScratchOffButton();
        }

        // protected void OnMouseDrag()
        // {
            // base.OnMouseDrag();

            //Card Follow Mouse
            // Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + (Vector3)dragOffset;
            // Vector2 cardToTarget = targetPos - (Vector2)transform.position;
            // transform.position += (Vector3)cardToTarget.normalized * MathF.Pow(cardToTarget.magnitude,1f) * dragSpeed * Time.deltaTime;
        
            //Adjust Buy Area
            // buyCardManager.AdjustBuyArea(this.transform);
        
            //Check if in buy area
            // float cardYPosOnViewport = Camera.main.WorldToViewportPoint(this.transform.position).y;
            // float buyAreaUpperEdgeYOnViewport = Camera.main.ScreenToViewportPoint(buyCardManager.buyArea.anchoredPosition).y;
            // if (buyAreaUpperEdgeYOnViewport > cardYPosOnViewport)
            // {
            //     isInBuyArea = true;
            //     // cardFaceSprite.DOColor(new Color(1,1,1,0.5f), 0.1f);
            // }
            // else
            // {
            //     isInBuyArea = false;
            //     // cardFaceSprite.DOColor(Color.gray, 0.1f);
            // }

        // }

        protected void OnMouseUp()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            //Scale
            cardFaceSprite.transform.DOScale(Vector3.one, 0.1f);
        }

        protected void OnMouseUpAsButton()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            // isDragging = false;

            // base.OnMouseUp();
            
            //Order
            // cardFaceSprite.sortingOrder = 0;
        
            //Deactivate Buy Area
            // buyCardManager.DeactivateBuyArea();
            
            //Activate ScratchOff Button
            // buyCardManager.ActivateScratchOffButton();
        
            //Check if buy
            // if (isInBuyArea)
            // {
            //     buyCardManager.TryBuyCard(this);
            // }

            if (!isCardSelected) PickCard();
        }

        protected void OnMouseExit()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            // if (!isDragging)
            // {
            //     base.OnMouseExit();
            foreach (var shadow in shadows)
            {
                SpriteRenderer shadowSprite = shadow.transform.Find("Shadow").GetComponent<SpriteRenderer>();
                shadowSprite.material.DOColor(shadow.shadowColor, 0.1f);
            }
            // }
        }
    }
}
