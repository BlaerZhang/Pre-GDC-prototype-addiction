using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Interaction;
using ScratchCardGeneration;
using ScratchCardGeneration.LayoutConstructor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Manager
{
    public class BuyCardManager : MonoBehaviour
    {
        // public static BuyCardManager instance;
    
        [Header("Spawn Cards")]
        public List<Draggable> cardsToBuy = new List<Draggable>();
        public List<Transform> cardSpawnPos;
        public Transform cardPurchasePos;
        public Draggable cardPrefab;

        [Header("Deal Cards Feedbacks")] 
        public bool dealAudio = true;
        public List<AudioClip> dealSounds;

        [Header("Buy Cards")] 
        public int price;
        
        [Header("Buy Feedbacks")] 
        public bool buyAudio = true;
        public List<AudioClip> buySounds;
        public bool buyParticle = true;
        public List<ParticleSystem> buyParticles;
        public bool zoomInAudio = true;
        public List<AudioClip> zoomInSounds;
        public bool zoomInParticle = true;
        public List<ParticleSystem> zoomInParticles;

        [Header("Hand")] 
        public Animator handAnimator;
    
        [Header("Buy Area")] 
        public RectTransform buyArea;
        [Range(0,1)] public float areaActivateThreshold = 0.5f;
        public float areaActivateDistance = 200;
        [Range(0,1)] public float areaStopThreshold = 0.2f;
        public float areaMoveAmount = 1000;
        
        [Header("Give Card Feedbacks")] 
        public bool giveAudio = true;
        public List<AudioClip> giveSounds;
        public bool giveParticle = true;
        public List<ParticleSystem> giveParticles;
    
        [Header("Give Area")] 
        public RectTransform giveArea;
        [Range(0,1)] public float giveAreaActivateThreshold = 0.5f;
        public float giveAreaActivateDistance = 200;
        [Range(0,1)] public float giveAreaStopThreshold = 0.2f;
        public float giveAreaMoveAmount = 1000;

        [Header("Scratching")] 
        public bool isScratching = false;
        private ScratchCardGenerator generator;
    
        [Header("Move to Scratch-off")] 
        public RectTransform scratchOffButton;

        public static Action<ScratchCardBrand, int, float, Vector3> onScratchCardSelected; //Buy Card Action
        public static Action onChangeSubmissionStatus; //Give Card Action
        public static Action<float> onSubmitScratchCard;
    

        private void Awake()
        {
            // instance = this;
            generator = GameManager.Instance.scratchCardGenerator;
        }

        private void OnEnable()
        {
            GameManager.Instance.switchSceneManager.onSceneChange += OnSceneChange;
        }

        private void OnDisable()
        {
            GameManager.Instance.switchSceneManager.onSceneChange -= OnSceneChange;
        }

        private void OnSceneChange(string sceneLoaded)
        {
            if (sceneLoaded == "Buy Card")
            {
                SpawnCardsToBuy();
                ActivateScratchOffButton();
            }
        }

        public void sortCardsOrder()
        {
            if (cardsToBuy.Count <= 0) return;
            for (int i = 0; i < cardsToBuy.Count; i++)
            {
                cardsToBuy.ToArray()[i].transform.DOMoveZ(0.1f * i, 0);
            }
        }

        public void SpawnCardsToBuy()
        {
            price = GameManager.Instance.lastPickPrice;
            GameManager.Instance.uiManager.UpdateBuyPrice(price);
        
            for (int i = 0; i < 5; i++)
            {
                Draggable cardInstance = Instantiate(cardPrefab, new Vector3(100,100), Quaternion.identity);
                cardsToBuy.Add(cardInstance);
            }

            StartCoroutine(DealCards());
        }
        
        IEnumerator DealCards()
        {
            handAnimator.SetTrigger("Deal");
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < 5; i++)
            {
                cardsToBuy[i].transform.position = cardSpawnPos[i].position;
                sortCardsOrder();
                if (dealAudio && dealSounds.Count > 0)
                    GameManager.Instance.audioManager.PlaySound(dealSounds[Random.Range(0, dealSounds.Count)]);
                yield return new WaitForSeconds(0.09f);
            }
        }

        public void BuyCard(Draggable card)
        {
            if (price <= GameManager.Instance.GetComponent<ResourceManager>().PlayerGold)
            {
                //set gold
                GameManager.Instance.GetComponent<ResourceManager>().PlayerGold -= price;
            
                //disable collider
                card.GetComponent<BoxCollider2D>().enabled = false;
            
                //remove from list
                cardsToBuy.Remove(card);
            
                //move to purchase pos
                card.transform.DOMove(cardPurchasePos.position, 0.1f);
                card.transform.DORotate(cardPurchasePos.rotation.eulerAngles, 0.1f);
                
                //feedback
                if (buyAudio && buySounds.Count > 0)
                    GameManager.Instance.audioManager.PlaySound(buySounds[Random.Range(0, buySounds.Count)]);

                if (buyParticle && buyParticles.Count > 0)
                {
                    //TODO: Particle Effect
                }
            
                //start collect + zoom
                StartCoroutine(BuyCardCoroutineChain(true, card));
            }
            else
            {
                GameManager.Instance.uiManager.PlayNotEnoughGoldAnimation();
            }
        }

        IEnumerator CollectCards(bool isPurchased)
        {
            //cards move to slots & disable drag
            for (int i = 0; i < cardsToBuy.Count; i++)
            {
                cardsToBuy[i].transform.DOMoveX(cardSpawnPos[i].position.x, 0.1f);
                cardsToBuy[i].transform.DOMoveY(cardSpawnPos[i].position.y, 0.1f);
                cardsToBuy[i].GetComponent<BoxCollider2D>().enabled = false;
            }
        
            //play hand animation
            handAnimator.SetTrigger("Collect");
        
            yield return new WaitForSeconds(0.42f);

            int numberOfCardsToCollect = cardsToBuy.Count;
        
            //destroy cards with interval
            for (int i = 0; i < numberOfCardsToCollect; i++)
            {
                GameObject cardToCollect = cardsToBuy[0].gameObject;
                cardsToBuy.Remove(cardsToBuy[0]);
                Destroy(cardToCollect);
                
                if (dealAudio && dealSounds.Count > 0)
                    GameManager.Instance.audioManager.PlaySound(dealSounds[Random.Range(0, dealSounds.Count)]);
                
                yield return new WaitForSeconds(0.11f);
            }
        }

        public void ZoomInCard(Draggable card)
        {
            //set initial pos
            card.transform.rotation = Quaternion.Euler(Vector3.zero);
        
            //set order
            card.cardSprite.sortingOrder = 6;
        
            Sequence zoomInCardAnimation = DOTween.Sequence();
            zoomInCardAnimation
                .Append(card.cardSprite.DOFade(0, 0))
                .Append(card.transform.DOMove(this.transform.position,0))
                .Append(card.transform.DOScale(Vector3.one * 10f, 0))
                .Append(card.cardSprite.DOFade(1, 0.1f))
                .Insert(0, card.transform.DOScale(Vector3.one * 1.2f, 0.5f)).SetEase(Ease.OutQuad)
                .AppendInterval(0.5f)
                .Append(card.transform.DOScale(Vector3.one, 0.1f)).SetEase(Ease.Linear)
                .OnComplete((() =>
                {
                    //set action to generate card
                    onScratchCardSelected?.Invoke(ScratchCardBrand.Fruities, (int)GameManager.Instance.lastPickTier, GameManager.Instance.lastPickPrice, transform.position);
                    
                    //Feedback
                    if (zoomInAudio && zoomInSounds.Count > 0)
                        GameManager.Instance.audioManager.PlaySound(zoomInSounds[Random.Range(0, zoomInSounds.Count)]);
                    if (zoomInParticle && zoomInParticles.Count > 0) ; //TODO: Zoom In Particles
                    
                    print("Action Invoked");
                    
                    Destroy(card.gameObject);
                }));
            
            // .AppendInterval(0.25f)
            // .Append(card.transform.DOShakeRotation(0.25f, new Vector3(0, 0, 2.5f), 50, 300F));

            zoomInCardAnimation.Play();
        }
    
        IEnumerator BuyCardCoroutineChain(bool isPurchased, Draggable card)
        {
            DeactivateScratchOffButton();
            yield return StartCoroutine(CollectCards(isPurchased));
            ZoomInCard(card);
        }
    
        public void GiveCard()
        {
            GameObject card = generator.currentScratchCard;
            
            //set action
            onChangeSubmissionStatus?.Invoke();
            onSubmitScratchCard?.Invoke(generator.currentCardPrize);

            card.transform.DOMoveY(card.transform.position.y + 1, 0.2f);
            card.transform.DOScale(0.9f, 0.2f);
            card.transform.DOScale(0.9f, 3f).OnComplete((() =>
            {
                GameManager.Instance.GetComponent<ResourceManager>().PlayerGold += (int)generator.currentCardPrize;
                card.transform.DOMoveY(card.transform.position.y + 10, 0.1f).OnComplete((() =>
                {
                    //Reset
                    SpawnCardsToBuy();
                    ActivateScratchOffButton();
                    Destroy(card.gameObject);
                }));
            
                //Feedback
                if (generator.currentCardPrize <= 0) return;
                if (giveAudio && giveSounds.Count > 0)
                    GameManager.Instance.audioManager.PlaySound(giveSounds[Random.Range(0, giveSounds.Count)]);
                if (giveParticle && giveParticles.Count > 0) ; //TODO: Give Particles
            }));
        }

        public void AdjustGiveArea(Transform cardPos)
        {
            float cardYPosOnViewport = Camera.main.WorldToViewportPoint(cardPos.position).y;
        
            if(cardYPosOnViewport > giveAreaActivateThreshold && cardYPosOnViewport < giveAreaStopThreshold)
                giveArea.anchoredPosition = new Vector2(giveArea.anchoredPosition.x, -giveAreaActivateDistance + (giveAreaActivateThreshold - cardYPosOnViewport) * giveAreaMoveAmount);
        }

        public void AdjustBuyArea(Transform cardPos)
        {
            float cardYPosOnViewport = Camera.main.WorldToViewportPoint(cardPos.position).y;
        
            if(cardYPosOnViewport < areaActivateThreshold && cardYPosOnViewport > areaStopThreshold)
                buyArea.anchoredPosition = new Vector2(buyArea.anchoredPosition.x, areaActivateDistance + (areaActivateThreshold - cardYPosOnViewport) * areaMoveAmount);
        }
    
        public void CollectCard(bool isPurchased)
        {
            StartCoroutine(CollectCards(isPurchased));
        }

        public void ActivateBuyArea()
        {
            buyArea.DOAnchorPosY(areaActivateDistance, 0.1f);
        }
    
        public void DeactivateBuyArea()
        {
            buyArea.DOAnchorPosY(0, 0.1f);
        }
    
        public void ActivateGiveArea()
        {
            giveArea.DOAnchorPosY(-giveAreaActivateDistance, 0.1f);
        }
    
        public void DeactivateGiveArea()
        {
            giveArea.DOAnchorPosY(0, 0.1f);
        }
    
        public void ActivateScratchOffButton()
        {
            scratchOffButton.gameObject.SetActive(true);
            scratchOffButton.DOAnchorPosX(125, 0.1f);
        }

        public void DeactivateScratchOffButton()
        {
            scratchOffButton.DOAnchorPosX(-200, 0.1f);
        }
    }
}
