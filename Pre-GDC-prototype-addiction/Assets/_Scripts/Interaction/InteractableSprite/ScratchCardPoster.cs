using System;
using _Scripts.FaceEventSystem;
using _Scripts.FaceEventSystem.FaceEvents;
using _Scripts.Interaction.PosterPicking;
using _Scripts.Manager;
using _Scripts.ScratchCardGeneration;
using _Scripts.VisualTools;
using Abu;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace _Scripts.Interaction.InteractableSprite
{
    public class ScratchCardPoster : InteractableSpriteBase
    {
        public static bool isInteractable = true;

        [Title("Card")]
        public bool unlock = false;
        public Sprite lockedSprite;
        public int price = 1;
        public ScratchCardBrand cardBrand = ScratchCardBrand.Fruities;
        [HideInInspector] public int originalPrice;
        private int discountPrice;
      
        public ScratchCardTier tier = ScratchCardTier.Level1;
        
        [Title("Drag")]
        public float dragSpeed = 25;
        public bool isDragging = false;
        public bool isInPickArea = false;

        [Title("Face Event")]
        public bool discount = false;
        public ScratchCardTier eventTriggerTier;
        public float discountPriceTagYPos;
        private float originalPriceTagYPos;
    
        [Title("Feedback")]
        public float hoverScale = 0.95f;

        private SpriteRenderer cardSprite;
        private SpriteRenderer pricePanelSprite;
        private SpriteShadow[] shadows;
        private SortingGroup sortingGroup;
        private TextMeshPro priceText;
        private SpriteRenderer discountCross;
        
        private Vector2 dragOffset = new Vector2(0, 0);
        private Vector2 originalLocalPosition;

        public static Action onPosterDragged;
        public static Func<Vector2, bool> onPosterReleased;
        public static Action<ScratchCardPoster, bool> onTryBuyPoster;

        private TutorialHighlight[] highlights;

        void Start()
        {
            cardSprite = transform.Find("Poster Sprite").GetComponent<SpriteRenderer>();
            pricePanelSprite = transform.Find("Poster Sprite/Price Panel Sprite").GetComponent<SpriteRenderer>();
            discountCross = transform.Find("Poster Sprite/Price Panel Sprite/Cross").GetComponent<SpriteRenderer>();
            shadows = GetComponentsInChildren<SpriteShadow>();
            sortingGroup = GetComponent<SortingGroup>();
            priceText = GetComponentInChildren<TextMeshPro>();
            highlights = GetComponentsInChildren<TutorialHighlight>();
            foreach (var highlight in highlights) { highlight.enabled = false; }
            
            isDragging = false;
            originalLocalPosition = transform.localPosition;
            originalPriceTagYPos = pricePanelSprite.transform.localPosition.y;
            priceText.text = $"${price}";
            
            DetectDiscount();
            
            if (!unlock)
            {
                cardSprite.sprite = lockedSprite;
                pricePanelSprite.gameObject.SetActive(false);
                discountCross.enabled = false;
                priceText.enabled = false;
            }
        }
        
        private void OnEnable()
        {
            FaceEventManager.onFaceEventDurationReduced += DetectDiscount;
            PosterHolder.onRecoverHolderPosition += DisplayDiscount;
            originalPrice = price;
        }

        private void OnDisable()
        {
            FaceEventManager.onFaceEventDurationReduced -= DetectDiscount;
            PosterHolder.onRecoverHolderPosition -= DisplayDiscount;
        }

        private void DetectDiscount()
        {
            ResetDiscountDisplay();
            
            discount = GameManager.Instance.faceEventManager.faceEventDurationDict[FaceEventType.Discount] > 0;

            if (!discount)
            {
                price = originalPrice;
                ResetDiscountDisplay();
                return;
            }
            
            //calculate discount price
            int tierDistance = tier - GameManager.Instance.faceEventManager.discountTriggerTier;
            discountPrice = GameManager.Instance.faceEventManager.CalculateDiscount(tierDistance, originalPrice);
            price = discountPrice;
        }

        private void DisplayDiscount()
        {
            if (!discount || !unlock) return;
            
            pricePanelSprite.transform.DOLocalMoveY(discountPriceTagYPos, 0.5f).SetEase(Ease.OutElastic);
            discountCross.enabled = true;
            priceText.text = $"${price}\n${originalPrice}";
        }

        private void ResetDiscountDisplay()
        {
            if (!unlock) return;

            pricePanelSprite.transform.DOLocalMoveY(originalPriceTagYPos, 0);
            discountCross.enabled = false;
            priceText.text = $"${price}";
        }

        protected override void OnMouseEnter()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            // if (!isInteractable) return;

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
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (!isInteractable) return;

            if (unlock)
            {
                isDragging = true;

                foreach (var highlight in highlights) { highlight.enabled = true; }
                HighlightManager.SmoothEnableHighlight(0.05f);
                
                base.OnMouseDown();
        
                //Sort order
                transform.DOMoveZ(-0.1f, 0);
                // sortingGroup.sortingOrder = 10;
        
                //Scale
                cardSprite.transform.DOScale(Vector3.one * hoverScale, 0.1f);
        
                //Set Drag Point Offset
                dragOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
                //Activate Pick Area
                onPosterDragged?.Invoke();
                // MenuManager.instance.ActivatePickArea();
                
                //Deactivate Incremental Button
                // MenuManager.instance.DeactivateIncrementalButton();
            }
            else
            {
                //play locked animation
            }
        }

        protected override void OnMouseDrag()
        {
            if (EventSystem.current.IsPointerOverGameObject() && !isDragging) return;
            if (!isInteractable) return;

            if (!unlock) return;

            base.OnMouseDrag();
            
            //Card Follow Mouse
            Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + (Vector3)dragOffset;
            Vector2 cardToTarget = targetPos - (Vector2)transform.position;
            transform.position += (Vector3)cardToTarget.normalized * MathF.Pow(cardToTarget.magnitude,1f) * dragSpeed * Time.deltaTime;
        
            //Adjust Buy Area
            // MenuManager.instance.AdjustPickArea(this.transform);
        
            // Check if in buy area
            // float cardXPosOnViewport = Camera.main.WorldToViewportPoint(this.transform.position).x;
            // float pickAreaLeftEdgeXOnViewport = Camera.main.ScreenToViewportPoint(MenuManager.instance.pickArea.anchoredPosition).x;
            // if (1 + pickAreaLeftEdgeXOnViewport < cardXPosOnViewport)
            // {
            //     isInPickArea = true;
            //     // cardSprite.DOColor(new Color(1,1,1,0.5f), 0.1f);
            // }
            // else
            // {
            //     isInPickArea = false;
            //     // cardSprite.DOColor(Color.gray, 0.1f);
            // }
        }
        protected override void OnMouseUp()
        {
            if (!isDragging) return;
            
            if (!isInteractable) return;

            if(!unlock) return;
            
            isDragging = false;
            
            foreach (var highlight in highlights) { highlight.enabled = false; }
            HighlightManager.SmoothDisableHighlight(0.05f);
            
            base.OnMouseUp();
        
            //Scale
            cardSprite.transform.DOScale(Vector3.one, 0.1f);
        
            //Reset Order
            transform.DOMoveZ(0f, 0);
            sortingGroup.sortingOrder = 0;

            // if in picking area
            if (onPosterReleased?.Invoke(transform.position) == true)
            {
                isInPickArea = true;

                if (price <= GameManager.Instance.resourceManager.PlayerGold)
                {
                    onTryBuyPoster?.Invoke(this, true);
                    print("picked");
                }
                else
                {
                    onTryBuyPoster?.Invoke(this, false);
                    print("not enough money");
                    GameManager.Instance.uiManager.PlayNotEnoughGoldAnimation();
                }
            }

            //Go back
            transform.DOLocalMove(originalLocalPosition, 0.1f)
            .OnStart(() =>
            {
                if (isInPickArea) transform.GetChild(0).gameObject.SetActive(false);
            })
            .OnComplete(() =>
            {
                if (isInPickArea)
                {
                    transform.GetChild(0).gameObject.SetActive(true);
                    isInPickArea = false;
                }
            });

            //Deactivate Pick Area
            // MenuManager.instance.DeactivatePickArea();

            //Activate Incremental Button
            // if (!GameManager.Instance.incrementalLock) MenuManager.instance.ActivateIncrementalButton();

            //Check if pick
            // if (isInPickArea) MenuManager.instance.PickCard(this);
        }

        protected override void OnMouseExit()
        {
            // if (EventSystem.current.IsPointerOverGameObject()) return;
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
