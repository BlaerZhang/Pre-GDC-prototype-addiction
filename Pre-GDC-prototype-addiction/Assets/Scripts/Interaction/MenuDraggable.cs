using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interaction
{
    public class MenuDraggable : MonoBehaviour
    {
        [Header("Drag")]
        public float dragSpeed = 25;
        public bool isDragging = false;
        [FormerlySerializedAs("isInBuyArea")] public bool isInPickArea = false;
    
        [Header("Feedback")] 
        public float hoverScale = 0.95f;

        private SpriteRenderer cardSprite;
        private Vector2 dragOffset = new Vector2(0, 0);
        private Vector2 originalLocalPos;
    
        void Start()
        {
            cardSprite = GetComponentInChildren<SpriteRenderer>();
            isDragging = false;
            originalLocalPos = transform.localPosition;
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
        
            //Sort order
            transform.DOMoveZ(-0.1f, 0);
        
            //Scale
            cardSprite.transform.DOScale(Vector3.one * hoverScale, 0.1f);
        
            //Set Drag Point Offset
            dragOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
            //Activate Pick Area
            MenuManager.instance.pickArea.DOAnchorPosX(-200, 0.1f);
        }

        private void OnMouseDrag()
        {
            //Card Follow Mouse
            Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + (Vector3)dragOffset;
            Vector2 cardToTarget = targetPos - (Vector2)transform.position;
            transform.position += (Vector3)cardToTarget.normalized * MathF.Pow(cardToTarget.magnitude,1f) * dragSpeed * Time.deltaTime;
        
            //Adjust Buy Area
            float cardXPosOnViewport = Camera.main.WorldToViewportPoint(this.transform.position).x;
            switch (cardXPosOnViewport)
            {
                case > 0.8f:
                    break;
                case > 0.5f:
                    MenuManager.instance.pickArea.anchoredPosition =
                        new Vector2(-200 - (cardXPosOnViewport - 0.5f) * 1000,
                            MenuManager.instance.pickArea.anchoredPosition.y);
                    break;
                case <= 0.5f:
                    break;
            }
        
            // Check if in buy area
            float pickAreaLeftEdgeXOnViewport = Camera.main.ScreenToViewportPoint(MenuManager.instance.pickArea.anchoredPosition).x;
            if (1 + pickAreaLeftEdgeXOnViewport < cardXPosOnViewport)
            {
                isInPickArea = true;
                cardSprite.DOColor(Color.yellow, 0.1f);
            }
            else
            {
                isInPickArea = false;
                cardSprite.DOColor(Color.gray, 0.1f);
            }
            print(1+pickAreaLeftEdgeXOnViewport);

        }
        private void OnMouseUp()
        {
            isDragging = false;
        
            //Scale
            cardSprite.transform.DOScale(Vector3.one, 0.1f);
        
            //Reset Order
            transform.DOMoveZ(0f, 0);
        
            //Deactivate Pick Area
            MenuManager.instance.pickArea.DOAnchorPosX(0, 0.1f);
        
            //Check if pick
            if (isInPickArea) MenuManager.instance.PickCard(this);
            else transform.DOLocalMove(originalLocalPos, 0.1f);
        }

        private void OnMouseExit()
        {
            if (!isDragging) cardSprite.DOColor(Color.white, 0.1f);
        }
    }
}
