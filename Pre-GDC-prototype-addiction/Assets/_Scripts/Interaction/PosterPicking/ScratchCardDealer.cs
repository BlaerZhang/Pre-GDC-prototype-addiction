using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Manager;
using ScratchCardGeneration;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace Interaction
{
    public class ScratchCardDealer : MonoBehaviour
    {
        public static int currentPickedCardPrice = 1;
        public static int currentPickedCardOriginalPrice = 1;
        public static ScratchCardTier currentPickedCardTier = ScratchCardTier.Level1;

        public static Action<ScratchCardBrand, int, int, Sprite> onToScratchStage;

        [Title("Card Dealing Settings")]
        [SerializeField] private Vector2 cardSpawnPosition;
        [SerializeField] private float dealCardDuration;
        [SerializeField] private List<Vector2> cardSlotPositions;

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
        }

        private void OnDisable()
        {
            ScratchCardPoster.onTryBuyPoster -= SpawnCardsToBuy;
            SelectableScratchCard.onScratchCardSelected -= TransitionToScratching;
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
            cardsToSelect.Remove(selectedCard);
            selectedCard.transform.position = new Vector2(0, 0.5f);

            // spawn card
            Sequence zoomInCardAnimation = DOTween.Sequence();
            zoomInCardAnimation
                .Append(selectedCard.cardFaceSprite.DOFade(0, 0))
                .Append(selectedCard.transform.DOScale(Vector3.one * 10f, 0))
                .Append(selectedCard.cardFaceSprite.DOFade(1, 0.1f))
                .Insert(0, selectedCard.transform.DOScale(Vector3.one * 1.2f, 0.5f)).SetEase(Ease.OutQuad)
                .AppendInterval(0.5f)
                .Append(selectedCard.transform.DOScale(Vector3.one, 0.1f)).SetEase(Ease.Linear)
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

        #region Redeem

        private void RedeemPrize()
        {

        }

        private void ShowRedeemArea()
        {

        }

        #endregion
    }
}