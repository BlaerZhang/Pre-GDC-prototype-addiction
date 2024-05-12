using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.FaceEventSystem;
using _Scripts.Interaction.InteractableSprite;
using _Scripts.Manager;
using _Scripts.ScratchCardGeneration;
using _Scripts.ScratchCardGeneration.LayoutConstructor;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Interaction.PosterPicking
{
    public class ScratchCardDealer : MonoBehaviour
    {
        public static int currentPickedCardPrice = 1;
        public static int currentPickedCardOriginalPrice = 1;
        public static ScratchCardTier currentPickedCardTier = ScratchCardTier.Level1;

        public static Action<ScratchCardBrand, int, int, Sprite> onToScratchStage;
        public static Action onChangeSubmissionStatus; //Give Card Action
        public static Action<float> onSubmitScratchCard;

        [Title("Card Dealing Settings")]
        [SerializeField] private Vector2 cardSpawnPosition;
        [SerializeField] private float dealCardDuration;
        [SerializeField] private List<Vector2> cardSlotPositions;
        [SerializeField] private float cardZoomEndScale = 1f;

        [Title("Redeem")]
        [SerializeField] private RectTransform redeemArea;
        [Range(0,1)] public float redeemAreaActivateThreshold = 0.5f;
        public float redeemAreaActivateDistance = 200;
        [Range(0,1)] public float redeemAreaStopThreshold = 0.2f;
        public float redeemAreaMoveAmount = 1000;
        public float redeemCardStayYOffset;

        public static Action onPrizeRedeemed;

        [Title("Feedback")] 
        [SerializeField] private bool dealAudio = true;
        [SerializeField] private List<AudioClip> dealSounds;

        private ScratchCardBrand currentCardBrand;
        private List<SelectableScratchCard> cardsToSelect = new();
        private int cardSpawnCount;

        private void Start()
        {
            cardSpawnCount = cardSlotPositions.Count;
        }

        private void OnEnable()
        {
            ScratchCardPoster.onTryBuyPoster += SpawnCardsToBuy;
            SelectableScratchCard.onScratchCardSelected += TransitionToScratching;

            ScratchDraggable.onScratchCardClicked += ActivateRedeemArea;
            ScratchDraggable.onScratchCardDragging += AdjustRedeemArea;
            ScratchDraggable.onScratchCardReleased += OnScratchCardReleased;
        }

        private void OnDisable()
        {
            ScratchCardPoster.onTryBuyPoster -= SpawnCardsToBuy;
            SelectableScratchCard.onScratchCardSelected -= TransitionToScratching;

            ScratchDraggable.onScratchCardClicked -= ActivateRedeemArea;
            ScratchDraggable.onScratchCardDragging -= AdjustRedeemArea;
            ScratchDraggable.onScratchCardReleased -= OnScratchCardReleased;
        }

        #region Deal Cards
        private void SpawnCardsToBuy(ScratchCardPoster cardPoster, bool isBought)
        {
            if (!isBought) return;

            currentCardBrand = cardPoster.cardBrand;
            currentPickedCardPrice = cardPoster.price;
            currentPickedCardOriginalPrice = cardPoster.originalPrice;
            currentPickedCardTier = cardPoster.tier;

            // GameManager.Instance.uiManager.UpdateBuyPrice(price); //update UI

            //spawn card
            foreach (var card in GameManager.Instance.cardPoolManager.CreateCardPool(currentPickedCardTier, cardSpawnCount))
            {
                SelectableScratchCard cardInstance = Instantiate(card, cardSpawnPosition, Quaternion.identity);

                cardInstance.gameObject.SetActive(false);
                cardsToSelect.Add(cardInstance);
            }

            DealCards(cardsToSelect);
            // StartCoroutine(DealCards());
        }

        private void DealCards(List<SelectableScratchCard> cardsToBuy)
        {
            // TODO: deal effects
            Sequence cardMoveSequence = DOTween.Sequence();

            for (int i = 0; i < cardSpawnCount; i++)
            {
                Transform spawnCard = cardsToBuy[i].transform;
                cardMoveSequence
                    .Append(spawnCard.DOMove(cardSlotPositions[i], dealCardDuration)
                        .OnStart(() =>
                        {
                            spawnCard.gameObject.SetActive(true);
                            // TODO: play sound
                            if (dealAudio && dealSounds.Count > 0)
                                GameManager.Instance.audioManager.PlaySound(dealSounds[Random.Range(0, dealSounds.Count)]);
                        }));
            }

            cardMoveSequence.Play();
        }
        #endregion

        #region Select -> Scratch
        private void TransitionToScratching(SelectableScratchCard selectedCard, FaceEventType faceEvent)
        {
            StartCoroutine(TransitionCoroutine(selectedCard, faceEvent));
        }


        private IEnumerator TransitionCoroutine(SelectableScratchCard selectedCard, FaceEventType faceEvent)
        {
            if (faceEvent != FaceEventType.NoEvent) yield return new WaitForSeconds(1);
            SpawnCard(selectedCard);

            CollectCards();
        }

        private void CollectCards()
        {
            foreach (var card in cardsToSelect)
            {
                // TODO: collect effects
                Destroy(card.gameObject);
            }
            cardsToSelect.Clear();
        }

        private void SpawnCard(SelectableScratchCard selectedCard)
        {
            if (!selectedCard) return;

            cardsToSelect.Remove(selectedCard);
            selectedCard.transform.position = GameManager.Instance.scratchCardGenerator.cardGenerationPosition;

            // spawn card
            Sequence zoomInCardAnimation = DOTween.Sequence();
            zoomInCardAnimation
                .Append(selectedCard.cardFaceSprite.DOFade(0, 0))
                .Append(selectedCard.transform.DOScale(Vector3.one * 10f, 0))
                .Append(selectedCard.cardFaceSprite.DOFade(1, 0.1f))
                .Insert(0, selectedCard.transform.DOScale(Vector3.one * 1.2f, 0.5f)).SetEase(Ease.OutQuad)
                .AppendInterval(0.5f)
                .Append(selectedCard.transform.DOScale(Vector3.one * cardZoomEndScale, 0.1f)).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    print("SpawnCard complete");
                    //set action to generate card

                    //Feedback
                    // if (zoomInAudio && zoomInSounds.Count > 0)
                    //     GameManager.Instance.audioManager.PlaySound(zoomInSounds[Random.Range(0, zoomInSounds.Count)]);
                    // if (zoomInParticle && zoomInParticles.Count > 0) ; //TODO: Zoom In Particles
                    onToScratchStage?.Invoke(currentCardBrand, (int)currentPickedCardTier,
                        currentPickedCardOriginalPrice, selectedCard.cardBGSprite.sprite);
                    print("Action Invoked");

                    //TODO:Temp change sprite, will be deleted in future
                    // if (faceEventTypeResult != FaceEventType.NoEvent) yield return new WaitForSeconds(1);
                    // yield return StartCoroutine(CollectCards(isPurchased));

                    // GameObject.Find("currentScratchCard/ScratchCardBackground(Clone)/ScratchCard(Clone)").GetComponent<ScratchCardManager>().ScratchSurfaceSprite = card.cardSprite.sprite;

                    Destroy(selectedCard.gameObject);
                });

            zoomInCardAnimation.Play();
        }
        #endregion

        #region Redeem Prize
        private void RedeemPrize()
        {
            ScratchCardGenerator generator = GameManager.Instance.scratchCardGenerator;
            GameObject currentCard = generator.currentScratchCard;

            //set action
            onChangeSubmissionStatus?.Invoke();
            onSubmitScratchCard?.Invoke(generator.currentCardPrize);

            currentCard.transform.DOMoveY(redeemCardStayYOffset, 0.2f);
            currentCard.transform.DOScale(0.9f, 0.2f);
            DOVirtual.DelayedCall(2, () =>
            {
                GameManager.Instance.resourceManager.PlayerGold += (int)generator.currentCardPrize;
                GameManager.Instance.statsTrackingManager.UpdatePricePrizeHistory(currentPickedCardOriginalPrice, (int)generator.currentCardPrize);
                GameManager.Instance.resourceManager.ChangeTime(5);
                DeactivateRedeemArea();
                currentCard.transform.DOMoveY(currentCard.transform.position.y + 10, 0.1f).OnComplete(() =>
                {
                    Destroy(currentCard.gameObject);

                    // back to the poster
                    onPrizeRedeemed?.Invoke();
                });
                //Feedback
                // if (generator.currentCardPrize <= 0) return;
                // if (giveAudio && giveSounds.Count > 0)
                //     GameManager.Instance.audioManager.PlaySound(giveSounds[Random.Range(0, giveSounds.Count)]);
                // if (giveParticle && giveParticles.Count > 0) ; //TODO: Give Particles
            }).Play();
        }

        private void OnScratchCardReleased(bool isRedeemed)
        {
            if (!isRedeemed) DeactivateRedeemArea();
            if (isRedeemed) RedeemPrize();
        }

        private Vector2 AdjustRedeemArea(Vector2 cardPosition)
        {
            float cardYPositionOnViewport = Camera.main.WorldToViewportPoint(cardPosition).y;

            if(cardYPositionOnViewport > redeemAreaActivateThreshold && cardYPositionOnViewport < redeemAreaStopThreshold)
                redeemArea.anchoredPosition = new Vector2(redeemArea.anchoredPosition.x, -redeemAreaActivateDistance + (redeemAreaActivateThreshold - cardYPositionOnViewport) * redeemAreaMoveAmount);

            return redeemArea.anchoredPosition;
        }

        private void ActivateRedeemArea()
        {
            redeemArea.DOAnchorPosY(-redeemAreaActivateDistance, 0.1f);
        }

        private void DeactivateRedeemArea()
        {
            redeemArea.DOAnchorPosY(0, 0.1f);
        }
        #endregion
    }
}