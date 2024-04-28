using System;
using DG.Tweening;
using Manager;
using ScratchCardGeneration;
using ScratchCardGeneration.PrizeGenerator;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Interaction
{
    public class MenuDraggable : DraggableBase
    {
        [Header("Card")] 
        public bool unlock = false;
        public Sprite lockedSprite;
        public int price = 1;
        public ScratchCardBrand cardBrand = ScratchCardBrand.Fruities;
        [HideInInspector] public int originalPrice;
        private int discountPrice;
      
        public ScratchCardTier tier = ScratchCardTier.Level1;
        
        [Header("Drag")]
        public float dragSpeed = 25;
        public bool isDragging = false;
        public bool isInPickArea = false;

        [Header("Face Event")] 
        public bool discount = false;
        public ScratchCardTier eventTriggerTier;
        public float discountPriceTagYPos;
    
        [Header("Feedback")] 
        public float hoverScale = 0.95f;

        private SpriteRenderer cardSprite;
        private SpriteRenderer pricePanelSprite;
        private SpriteShadow[] shadows;
        private SortingGroup sortingGroup;
        private TextMeshPro priceText;
        private SpriteRenderer discountCross;
        
        private Vector2 dragOffset = new Vector2(0, 0);
        private Vector2 originalLocalPos;
    
        void Start()
        {
            cardSprite = transform.Find("Poster Sprite").GetComponent<SpriteRenderer>();
            pricePanelSprite = transform.Find("Poster Sprite/Price Panel Sprite").GetComponent<SpriteRenderer>();
            discountCross = transform.Find("Poster Sprite/Price Panel Sprite/Cross").GetComponent<SpriteRenderer>();
            shadows = GetComponentsInChildren<SpriteShadow>();
            sortingGroup = GetComponent<SortingGroup>();
            priceText = GetComponentInChildren<TextMeshPro>();
            
            isDragging = false;
            originalLocalPos = transform.localPosition;
            priceText.text = $"${price}";
            
            InitDiscount();
            
            if (!unlock)
            {
                cardSprite.sprite = lockedSprite;
                pricePanelSprite.gameObject.SetActive(false);
                discountCross.enabled = false;
                priceText.enabled = false;
            }
        }

        private void InitDiscount()
        { 
            discount = GameManager.Instance.faceEventManager.faceEventDurationDict[FaceEventType.Discount] > 0
                ? true
                : false;

            discountCross.enabled = false;

            if (!discount) return;
            
            //calculate discount price
            int tierDistance = tier - GameManager.Instance.faceEventManager.discountTriggerTier;
            discountPrice = GameManager.Instance.faceEventManager.CalculateDiscount(tierDistance, originalPrice);
            price = discountPrice;
        }

        private void OnEnable()
        {
            SwitchSceneManager.onSceneChanged += DisplayDiscount;
            originalPrice = price;
        }

        private void OnDisable()
        {
            SwitchSceneManager.onSceneChanged -= DisplayDiscount;
        }

        private void DisplayDiscount(string sceneName)
        {
            if (sceneName != "Menu") return;
            if (!discount || !unlock) return;
            
            pricePanelSprite.transform.DOLocalMoveY(discountPriceTagYPos, 0.5f).SetEase(Ease.OutElastic);
            discountCross.enabled = true;
            priceText.text = $"${price}\n${originalPrice}"; }

        protected override void OnMouseEnter()
        {
            if (!isDragging)
            {
                foreach (var shadow in shadows)
                {
                    SpriteRenderer shadowSprite = shadow.transform.Find("Shadow").GetComponent<SpriteRenderer>();
                    shadowSprite.material.DOColor(shadow.hoverShadowColor, 0.1f);
                }
            }
        }

        protected override void OnMouseDown()
        {
            if (unlock)
            {
                isDragging = true;
                
                base.OnMouseDown();
        
                //Sort order
                transform.DOMoveZ(-0.1f, 0);
                // sortingGroup.sortingOrder = 10;
        
                //Scale
                cardSprite.transform.DOScale(Vector3.one * hoverScale, 0.1f);
        
                //Set Drag Point Offset
                dragOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
                //Activate Pick Area
                MenuManager.instance.ActivatePickArea();
                
                //Deactivate Incremental Button
                MenuManager.instance.DeactivateIncrementalButton();
            }
            else
            {
                //play locked animation
            }
           
        }

        protected override void OnMouseDrag()
        {
            if (!unlock) return;
            
            //Card Follow Mouse
            Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + (Vector3)dragOffset;
            Vector2 cardToTarget = targetPos - (Vector2)transform.position;
            transform.position += (Vector3)cardToTarget.normalized * MathF.Pow(cardToTarget.magnitude,1f) * dragSpeed * Time.deltaTime;
        
            //Adjust Buy Area
            MenuManager.instance.AdjustPickArea(this.transform);
        
            // Check if in buy area
            float cardXPosOnViewport = Camera.main.WorldToViewportPoint(this.transform.position).x;
            float pickAreaLeftEdgeXOnViewport = Camera.main.ScreenToViewportPoint(MenuManager.instance.pickArea.anchoredPosition).x;
            if (1 + pickAreaLeftEdgeXOnViewport < cardXPosOnViewport)
            {
                isInPickArea = true;
                // cardSprite.DOColor(new Color(1,1,1,0.5f), 0.1f);
            }
            else
            {
                isInPickArea = false;
                // cardSprite.DOColor(Color.gray, 0.1f);
            }

        }
        protected override void OnMouseUp()
        {
            if(!unlock) return;
            
            isDragging = false;
            
            base.OnMouseUp();
        
            //Scale
            cardSprite.transform.DOScale(Vector3.one, 0.1f);
        
            //Reset Order
            transform.DOMoveZ(0f, 0);
            sortingGroup.sortingOrder = 0;
        
            //Deactivate Pick Area
            MenuManager.instance.DeactivatePickArea();
            
            //Activate Incremental Button
            if (!GameManager.Instance.incrementalLock) MenuManager.instance.ActivateIncrementalButton();
        
            //Check if pick
            if (isInPickArea) MenuManager.instance.PickCard(this);
            
            //Go back
            transform.DOLocalMove(originalLocalPos, 0.1f);
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
    }
}
