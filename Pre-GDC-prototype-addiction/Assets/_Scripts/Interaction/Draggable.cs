using System;
using DG.Tweening;
using Manager;
using UnityEngine;

namespace Interaction
{
    public class Draggable : DraggableBase
    {
        [Header("Drag")]
        public float dragSpeed = 1;
        public bool isDragging = false;
        public bool isInBuyArea = false;
    
        [Header("Feedback")] 
        public float hoverScale = 0.95f;

        [HideInInspector] public SpriteRenderer cardSprite;
        private Vector2 dragOffset = new Vector2(0, 0);

        private BuyCardManager buyCardManager;
    
        void Start()
        {
            cardSprite = GetComponentInChildren<SpriteRenderer>();
            buyCardManager = FindObjectOfType<BuyCardManager>();
            isDragging = false;
        }

        protected override void OnMouseEnter()
        {
            if (!isDragging) cardSprite.DOColor(Color.gray, 0.1f);
        }
        
        protected override void OnMouseDown()
        {
            isDragging = true;
            
            base.OnMouseDown();
        
            //Reorder in List and Sort order
            buyCardManager.cardsToBuy.Remove(this);
            buyCardManager.cardsToBuy.Insert(0,this);
            buyCardManager.sortCardsOrder();
            // cardSprite.sortingOrder = 10;
        
            //Scale
            cardSprite.transform.DOScale(Vector3.one * hoverScale, 0.1f);
        
            //Set Drag Point Offset
            dragOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
            //Activate Buy Area
            buyCardManager.ActivateBuyArea();
            
            //Deactivate ScratchOff Button
            buyCardManager.DeactivateScratchOffButton();
            
            
        }

        protected override void OnMouseDrag()
        {
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
                cardSprite.DOColor(new Color(1,1,1,0.5f), 0.1f);
            }
            else
            {
                isInBuyArea = false;
                cardSprite.DOColor(Color.gray, 0.1f);
            }

        }
        protected override void OnMouseUp()
        {
            isDragging = false;
            
            base.OnMouseUp();
        
            //Scale
            cardSprite.transform.DOScale(Vector3.one, 0.1f);
            
            //Order
            cardSprite.sortingOrder = 0;
        
            //Deactivate Buy Area
            buyCardManager.DeactivateBuyArea();
            
            //Activate ScratchOff Button
            buyCardManager.ActivateScratchOffButton();
        
            //Check if buy
            if (isInBuyArea)
            {
                buyCardManager.BuyCard(this);
            }
        }

        protected override void OnMouseExit()
        {
            if (!isDragging) cardSprite.DOColor(Color.white, 0.1f);
        }
    }
}
