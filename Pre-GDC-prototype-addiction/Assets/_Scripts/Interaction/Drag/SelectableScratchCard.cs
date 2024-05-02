using System;
using DG.Tweening;
using Manager;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interaction
{
    public class SelectableScratchCard : DraggableBase
    {
        [Header("Drag")]
        public float dragSpeed = 1;
        private bool isDragging = false;
        private bool isInBuyArea = false;
    
        [Header("Feedback")] 
        public float hoverScale = 0.95f;
        
        [Header("Metaphysics")]
        public FaceType faceType;

        [HideInInspector] public SpriteRenderer cardFaceSprite;
        [HideInInspector] public SpriteRenderer cardBGSprite;
        [FormerlySerializedAs("shadowSprites")] [HideInInspector] public SpriteShadow[] shadows;
        private Vector2 dragOffset = new Vector2(0, 0);

        private BuyCardManager buyCardManager;
    
        void Start()
        {
            cardFaceSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
            cardBGSprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
            shadows = GetComponentsInChildren<SpriteShadow>();
            buyCardManager = FindObjectOfType<BuyCardManager>();
            isDragging = false;
        }

        protected override void OnMouseEnter()
        {
            if (!isDragging)
            {
                base.OnMouseEnter();
                foreach (var shadow in shadows)
                {
                    SpriteRenderer shadowSprite = shadow.transform.Find("Shadow").GetComponent<SpriteRenderer>();
                    shadowSprite.material.DOColor(shadow.hoverShadowColor, 0.1f);
                }
            }
        }
        
        protected override void OnMouseDown()
        {
            isDragging = true;
            
            base.OnMouseDown();
        
            //Reorder in List and Sort order
            buyCardManager.cardsToBuy.Remove(this);
            buyCardManager.cardsToBuy.Insert(0,this);
            buyCardManager.SortCardsOrder();
            // cardSprite.sortingOrder = 10;
        
            //Scale
            cardFaceSprite.transform.DOScale(Vector3.one * hoverScale, 0.1f);
        
            //Set Drag Point Offset
            dragOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
            //Activate Buy Area
            buyCardManager.ActivateBuyArea();
            
            //Deactivate ScratchOff Button
            buyCardManager.DeactivateScratchOffButton();
            
            
        }

        protected override void OnMouseDrag()
        {
            base.OnMouseDrag();

            //Card Follow Mouse
            Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + (Vector3)dragOffset;
            Vector2 cardToTarget = targetPos - (Vector2)transform.position;
            transform.position += (Vector3)cardToTarget.normalized * MathF.Pow(cardToTarget.magnitude,1f) * dragSpeed * Time.deltaTime;
        
            //Adjust Buy Area
            buyCardManager.AdjustBuyArea(this.transform);
        
            //Check if in buy area
            float cardYPosOnViewport = Camera.main.WorldToViewportPoint(this.transform.position).y;
            float buyAreaUpperEdgeYOnViewport = Camera.main.ScreenToViewportPoint(buyCardManager.buyArea.anchoredPosition).y;
            if (buyAreaUpperEdgeYOnViewport > cardYPosOnViewport)
            {
                isInBuyArea = true;
                // cardFaceSprite.DOColor(new Color(1,1,1,0.5f), 0.1f);
            }
            else
            {
                isInBuyArea = false;
                // cardFaceSprite.DOColor(Color.gray, 0.1f);
            }

        }
        protected override void OnMouseUp()
        {
            isDragging = false;
            
            base.OnMouseUp();
        
            //Scale
            cardFaceSprite.transform.DOScale(Vector3.one, 0.1f);
            
            //Order
            cardFaceSprite.sortingOrder = 0;
        
            //Deactivate Buy Area
            buyCardManager.DeactivateBuyArea();
            
            //Activate ScratchOff Button
            buyCardManager.ActivateScratchOffButton();
        
            //Check if buy
            if (isInBuyArea)
            {
                buyCardManager.TryBuyCard(this);
            }
        }

        protected override void OnMouseExit()
        {
            if (!isDragging)
            {
                base.OnMouseExit();
                foreach (var shadow in shadows)
                {
                    SpriteRenderer shadowSprite = shadow.transform.Find("Shadow").GetComponent<SpriteRenderer>();
                    shadowSprite.material.DOColor(shadow.shadowColor, 0.1f);
                }
            }
        }
    }
}
