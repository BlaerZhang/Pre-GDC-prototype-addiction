using System;
using DG.Tweening;
using UnityEngine;

namespace Interaction
{
    public class Draggable : MonoBehaviour
    {
        [Header("Drag")]
        public float dragSpeed = 1;
        public bool isDragging = false;
        public bool isInBuyArea = false;
    
        [Header("Feedback")] 
        public float hoverScale = 0.95f;

        private SpriteRenderer cardSprite;
        private Vector2 dragOffset = new Vector2(0, 0);
    
        void Start()
        {
            cardSprite = GetComponentInChildren<SpriteRenderer>();
            isDragging = false;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnMouseEnter()
        {
            if (!isDragging) cardSprite.DOColor(Color.gray, 0.1f);
        }

        private void OnMouseDown()
        {
            isDragging = true;
        
            //Reorder in List and Sort order
            BuyCardManager.instance.cardsToBuy.Remove(this);
            BuyCardManager.instance.cardsToBuy.Insert(0,this);
            BuyCardManager.instance.sortCardsOrder();
        
            //Scale
            cardSprite.transform.DOScale(Vector3.one * hoverScale, 0.1f);
        
            //Set Drag Point Offset
            dragOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
            //Activate Buy Area
            BuyCardManager.instance.ActivateBuyArea();
        }

        private void OnMouseDrag()
        {
            //Card Follow Mouse
            Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + (Vector3)dragOffset;
            Vector2 cardToTarget = targetPos - (Vector2)transform.position;
            transform.position += (Vector3)cardToTarget.normalized * MathF.Pow(cardToTarget.magnitude,1f) * dragSpeed * Time.deltaTime;
        
            //Adjust Buy Area
            BuyCardManager.instance.AdjustBuyArea(this.transform);
        
            //Check if in buy area
            float cardYPosOnViewport = Camera.main.WorldToViewportPoint(this.transform.position).y;
            float buyAreaUpperEdgeYOnViewport = Camera.main.ScreenToViewportPoint(BuyCardManager.instance.buyArea.anchoredPosition).y;
            if (buyAreaUpperEdgeYOnViewport > cardYPosOnViewport)
            {
                isInBuyArea = true;
                cardSprite.DOColor(Color.yellow, 0.1f);
            }
            else
            {
                isInBuyArea = false;
                cardSprite.DOColor(Color.gray, 0.1f);
            }

        }
        private void OnMouseUp()
        {
            isDragging = false;
        
            //Scale
            cardSprite.transform.DOScale(Vector3.one, 0.1f);
        
            //Deactivate Buy Area
            BuyCardManager.instance.DeactivateBuyArea();
        
            //Check if buy
            if (isInBuyArea) BuyCardManager.instance.BuyCard(this);
        }

        private void OnMouseExit()
        {
            if (!isDragging) cardSprite.DOColor(Color.white, 0.1f);
        }
    }
}
