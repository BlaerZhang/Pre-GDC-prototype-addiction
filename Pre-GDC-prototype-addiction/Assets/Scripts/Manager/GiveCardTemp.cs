using System;
using System.Collections;
using DG.Tweening;
using ScratchCardGeneration.LayoutConstructor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class GiveCardTemp : MonoBehaviour
    {
        public ScratchCardGenerator generator;
        public GameObject hand;

        public static Action onChangeSubmissionStatus;
        public static Action<float> onSubmitScratchCard;

        public void GiveCard()
        {
            StartCoroutine(GiveCardAction());
        }

        private IEnumerator GiveCardAction()
        {
            generator = FindObjectOfType<ScratchCardGenerator>();

            onChangeSubmissionStatus();
            onSubmitScratchCard(generator.currentCardPrize);

            yield return new WaitForSeconds(1f);

            GameObject card = generator.transform.GetChild(0).gameObject;
            GameManager.Instance.GetComponent<ResourceManager>().PlayerGold += (int)generator.currentCardPrize;
            Sequence giveAnimation = DOTween.Sequence();
            giveAnimation
                .Append(card.transform.DOMoveY(card.transform.position.y + 1, 0.5f))
                .Append(hand.transform.DOMoveY(card.transform.position.y + 2, 1f))
                .AppendInterval(0.5f)
                .Append(hand.transform.DOMoveY(9, 1f))
                .Insert(2, card.transform.DOMoveY(9, 1f))
                .OnComplete((() => { SceneManager.LoadScene("Buy Card"); }));

            giveAnimation.Play();
        }
    }
}
