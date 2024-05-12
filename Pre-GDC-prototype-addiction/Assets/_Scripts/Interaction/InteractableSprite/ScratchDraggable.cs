using System;
using _Scripts.Interaction.PosterPicking;
using _Scripts.Manager;
using _Scripts.VisualTools;
using DG.Tweening;
using ScratchCardAsset;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.Interaction.InteractableSprite
{
    public class ScratchDraggable : InteractableSpriteBase
    {
        [Title("Drag")]
        public float dragSpeed = 1;
        public Vector3 scratchCardScale = Vector3.one;
        private bool isDragging = false;
        private bool dragLock = false;
        private bool isInRedeemArea = false;
    
        [Title("Feedback")]
        public SpriteRenderer cardSurfaceSprite;
        public SpriteRenderer cardBackgroundSprite;
        public float hoverScale = 1.05f;

        // private DetectScratchArea detectScratchArea;
        private Vector2 dragOffset = new Vector2(0, 0);
        private GameObject currentCard;
        // private Vector3 currentCardOriginalScale;
        private ScratchCardManager scratchCardManager;
        private BuyCardManager buyCardManager;
        private Vector2 initialPos;
        private SpriteShadow[] shadows;

        public static Action onScratchCardClicked;
        public static Func<Vector2, Vector2> onScratchCardDragging;
        public static Action<bool> onScratchCardReleased;

        void Start()
        {
            cardSurfaceSprite = GameObject.Find("Scratch Surface Sprite").GetComponent<SpriteRenderer>();
            cardBackgroundSprite = GameObject.Find("ScratchCardBackground(Clone)").GetComponent<SpriteRenderer>();
            // detectScratchArea = GetComponent<DetectScratchArea>();
            currentCard = GameObject.Find("currentScratchCard");
            currentCard.transform.localScale = scratchCardScale;
            // currentCardOriginalScale = currentCard.transform.localScale;
            // scratchCardManager = transform.parent.GetComponentInChildren<ScratchCardManager>();
            // buyCardManager = FindObjectOfType<BuyCardManager>();
            shadows = GetComponentsInChildren<SpriteShadow>();
            initialPos = transform.position;
            isDragging = false;
        }

        private void OnEnable()
        {
            ScratchCardDealer.onChangeSubmissionStatus += LockDrag;
        }

        private void OnDisable()
        {
            ScratchCardDealer.onChangeSubmissionStatus -= LockDrag;
        }

        private void Update()
        {
            // scratchCardManager.InputEnabled = !isDragging;
        }

        protected override void OnMouseEnter()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (!isDragging)
            {
                // print("Mouse Enter");
                foreach (var shadow in shadows)
                {
                    print(shadow);
                    SpriteRenderer shadowSprite = shadow.transform.Find("Shadow").GetComponent<SpriteRenderer>();
                    shadowSprite.material.DOColor(shadow.hoverShadowColor, 0.1f);
                }
            }
        }

        protected override void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (dragLock) return;
            
            isDragging = true;
            
            base.OnMouseDown();
        
            //Scale
            currentCard.transform.DOScale(scratchCardScale * hoverScale, 0.1f);
        
            //Set Drag Point Offset
            dragOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
            //Activate Give Area
            onScratchCardClicked?.Invoke();
            // buyCardManager.ActivateGiveArea();
        }

        protected override void OnMouseDrag()
        {
            if (EventSystem.current.IsPointerOverGameObject() && !isDragging) return;
            if (dragLock) return;
            
            //Card Follow Mouse Y
            Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + (Vector3)dragOffset;
            float cardToTarget = targetPosition.y - transform.position.y;
            currentCard.transform.position += new Vector3(0, MathF.Pow(cardToTarget,1f) * dragSpeed * Time.deltaTime,0);
            
            //Adjust Give Area
            Vector2 redeemArea = (Vector2)onScratchCardDragging?.Invoke(transform.position);
            // buyCardManager.AdjustGiveArea(this.transform);

            //Check if in give area
            float cardYPositionOnViewport = Camera.main.WorldToViewportPoint(transform.position).y;
            float giveAreaLowerEdgeYOnViewport = Camera.main.ScreenToViewportPoint(redeemArea).y;
            if (1 + giveAreaLowerEdgeYOnViewport < cardYPositionOnViewport)
            {
                isInRedeemArea = true;
                // TODO: change shadow color
                // cardSurfaceSprite.DOColor(new Color(1,1,1,0.5f), 0.1f);
                // cardBackgroundSprite.DOColor(new Color(1,1,1,0.5f), 0.1f);
            }
            else
            {
                isInRedeemArea = false;
                // cardSurfaceSprite.DOColor(Color.white, 0.1f);
                // cardBackgroundSprite.DOColor(Color.white, 0.1f);
            }
        }
        protected override void OnMouseUp()
        {
            if (!isDragging) return;
            if (dragLock) return;
            
            isDragging = false;
            
            base.OnMouseUp();
        
            //Scale
            currentCard.transform.DOScale(scratchCardScale, 0.1f);
        
            //Check if give
            if (isInRedeemArea)
            {
                onScratchCardReleased?.Invoke(true);
                // buyCardManager.GiveCard();
            }
            else
            {
                //Deactivate Give Area
                // buyCardManager.DeactivateGiveArea();
                onScratchCardReleased?.Invoke(false);
                currentCard.transform.DOMove(initialPos, 0.1f);
            }
        }

        protected override void OnMouseExit()
        {
            if (!isDragging)
            {
                foreach (var shadow in shadows)
                {
                    SpriteRenderer shadowSprite = shadow.transform.Find("Shadow").GetComponent<SpriteRenderer>();
                    shadowSprite.material.DOColor(shadow.shadowColor, 0.1f);    
                }
            }
        }

        private void LockDrag()
        {
            dragLock = true;
        }
    }
}
