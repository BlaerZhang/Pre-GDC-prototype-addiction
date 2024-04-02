using System;
using DG.Tweening;
using ScratchCardAsset;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interaction
{
    public class ScratchDraggable : MonoBehaviour
    {
        [Header("Drag")]
        public float dragSpeed = 1;
        public bool isDragging = false;
        public bool isInGiveArea = false;
    
        [Header("Feedback")] 
        public float hoverScale = 0.95f;

        public SpriteRenderer cardSurfaceSprite;
        public SpriteRenderer cardBackgroundSprite;
        private DetectScratchArea detectScratchArea;
        private Vector2 dragOffset = new Vector2(0, 0);
        private GameObject currentCard;
        private ScratchCardManager scratchCardManager;
    
        void Start()
        {
            cardSurfaceSprite = GameObject.Find("Scratch Surface Sprite").GetComponent<SpriteRenderer>();
            cardBackgroundSprite = GameObject.Find("ScratchCardBackground(Clone)").GetComponent<SpriteRenderer>();
            detectScratchArea = GetComponent<DetectScratchArea>();
            currentCard = GameObject.Find("newScratchCard");
            scratchCardManager = GetComponentInParent<ScratchCardManager>();
            isDragging = false;
        }

        private void Update()
        {
            scratchCardManager.InputEnabled = !isDragging;
        }

        private void OnMouseEnter()
        {
            if (!isDragging)
            {
                // cardSurfaceSprite.DOColor(Color.gray, 0.1f);
                // cardBackgroundSprite.DOColor(Color.gray, 0.1f);
            }
        }

        private void OnMouseDown()
        {
            isDragging = true;
        
            //Scale
            currentCard.transform.DOScale(Vector3.one * hoverScale, 0.1f);
        
            //Set Drag Point Offset
            dragOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
            //Activate Give Area
            BuyCardManager.instance.ActivateGiveArea();
        }

        private void OnMouseDrag()
        {
            //Card Follow Mouse Y
            Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + (Vector3)dragOffset;
            float cardToTarget = targetPos.y - transform.position.y;
            currentCard.transform.position += new Vector3(0, MathF.Pow(cardToTarget,1f) * dragSpeed * Time.deltaTime,0);
            
            //Adjust Give Area
            BuyCardManager.instance.AdjustGiveArea(this.transform);

            //Check if in give area
            float cardYPosOnViewport = Camera.main.WorldToViewportPoint(this.transform.position).y;
            float giveAreaLowerEdgeYOnViewport = Camera.main.ScreenToViewportPoint(BuyCardManager.instance.giveArea.anchoredPosition).y;
            print($"Area Low: {giveAreaLowerEdgeYOnViewport}");
            if (1 + giveAreaLowerEdgeYOnViewport < cardYPosOnViewport)
            {
                isInGiveArea = true;
                cardSurfaceSprite.DOColor(new Color(1,1,1,0.5f), 0.1f);
                cardBackgroundSprite.DOColor(new Color(1,1,1,0.5f), 0.1f);
            }
            else
            {
                isInGiveArea = false;
                cardSurfaceSprite.DOColor(Color.white, 0.1f);
                cardBackgroundSprite.DOColor(Color.white, 0.1f);
            }

        }
        private void OnMouseUp()
        {
            isDragging = false;
        
            //Scale
            currentCard.transform.DOScale(Vector3.one, 0.1f);
        
            //Deactivate Give Area
            BuyCardManager.instance.DeactivateGiveArea();
        
            //Check if give
            if (isInGiveArea)
            {
                BuyCardManager.instance.GiveCard();
            }
            else
            {
                currentCard.transform.DOLocalMove(Vector2.zero, 0.1f);
            }
        }

        private void OnMouseExit()
        {
            if (!isDragging)
            {
                // cardSurfaceSprite.DOColor(Color.white, 0.1f);
                // cardBackgroundSprite.DOColor(Color.white, 0.1f);
            }
        }
    }
}
