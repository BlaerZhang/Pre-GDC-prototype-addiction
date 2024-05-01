using DG.Tweening;
using Interaction;
using UnityEngine;

namespace Manager
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager instance;
    
        [Header("Spawn Cards")]
        // public List<Transform> cardSpawnPos;
        // public Draggable cardPrefab;

        [Header("Pick Cards")] 
        public Transform cardPurchasePos;

        [Header("Pick Area")] 
        public RectTransform pickArea;
        [Range(0,1)] public float areaActivateThreshold = 0.5f;
        public float areaActivateDistance = 200;
        [Range(0,1)] public float areaStopThreshold = 0.8f;
        public float areaMoveAmount = 1000;

        [Header("Move to Incremental")] 
        public RectTransform incrementalButton;
    

        private void Awake()
        {
            instance = this;
        }

        private void OnEnable()
        {
            SwitchSceneManager.onSceneChanged += InitializeButton;
        }

        private void OnDisable()
        {
            SwitchSceneManager.onSceneChanged -= InitializeButton;
        }
    
        public void PickCard(ScratchCardPoster card)
        {
            if (card.price <= GameManager.Instance.resourceManager.PlayerGold)
            {
                // card.transform.DOMove(cardPurchasePos.position, 0.1f);
                GameManager.Instance.lastPickPrice = card.price;
                GameManager.Instance.lastPickOriginalPrice = card.originalPrice;
                GameManager.Instance.lastPickTier = card.tier;
                GameManager.Instance.switchSceneManager.ChangeScene("Buy Card");
                GameManager.Instance.resourceManager.ChangeTime(3);
            }
            else
            {
                GameManager.Instance.uiManager.PlayNotEnoughGoldAnimation();
            }
      
        }

        public void AdjustPickArea(Transform posterPos)
        {
            float cardXPosOnViewport = Camera.main.WorldToViewportPoint(posterPos.position).x;

            if (cardXPosOnViewport < areaStopThreshold && cardXPosOnViewport > areaActivateThreshold) 
                pickArea.anchoredPosition = new Vector2(-areaActivateDistance - (cardXPosOnViewport - areaActivateThreshold) * areaMoveAmount, pickArea.anchoredPosition.y);
        }

        public void ActivatePickArea()
        {
            pickArea.gameObject.SetActive(true);
            pickArea.DOAnchorPosX(-areaActivateDistance, 0.1f);
        }

        public void DeactivatePickArea()
        {
            pickArea.DOAnchorPosX(600f, 0.1f).OnComplete((() => { pickArea.gameObject.SetActive(false); }));
        }
    
        public void ActivateIncrementalButton()
        {
            incrementalButton.gameObject.SetActive(true);
            incrementalButton.DOAnchorPosX(125, 0.1f);
        }

        public void DeactivateIncrementalButton()
        {
            incrementalButton.DOAnchorPosX(-200, 0.1f);
        }

        public void InitializeButton(string toScene)
        {
            if (toScene == "Menu")
            {
                ActivateIncrementalButton();
            }
        }
    }
}
