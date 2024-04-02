using System;
using System.Collections;
using Interaction;
using Manager;
using TMPro;
using UnityEngine;

namespace ScratchCardGeneration.Utilities
{
    public class NumberRoller : MonoBehaviour
    {
        public TMP_Text numberText;
        public float duration = 2f;
        private CanvasGroup canvasGroup;

        public TMP_FontAsset fontAsset;

        public static Action<TMP_FontAsset> onRollingEnds;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            numberText.font = fontAsset;
        }

        private void OnEnable()
        {
            PrizeRevealing.onPrizeRevealed += StartRolling;
            GiveCardTemp.onSubmitScratchCard += StartRolling;
        }

        private void OnDisable()
        {
            PrizeRevealing.onPrizeRevealed -= StartRolling;
            GiveCardTemp.onSubmitScratchCard -= StartRolling;
        }

        private void StartRolling(float prize)
        {
            StartCoroutine(FadeCanvasGroupAlpha(0, 1, 1f));
            StartCoroutine(RollNumber(prize));
            // onRollingEnds?.Invoke(fontAsset);
            onRollingEnds(fontAsset);
        }

        private IEnumerator RollNumber(float targetNumber)
        {
            float elapsed = 0f;
            int startNumber = 0;

            while (elapsed < 1 / duration)
            {
                elapsed += Time.deltaTime;
                float percentComplete = elapsed / duration;
                int currentNumber = (int)Mathf.Lerp(startNumber, targetNumber, percentComplete);
                numberText.text = currentNumber.ToString();
                yield return null;
            }

            numberText.text = targetNumber.ToString();

            yield return new WaitForSeconds(1f);
            StartCoroutine(FadeCanvasGroupAlpha(1, 0, 1f));
        }

        private IEnumerator FadeCanvasGroupAlpha(float startAlpha, float endAlpha, float duration)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;

            while (Time.time < endTime)
            {
                float elapsed = (Time.time - startTime) / duration;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed);
                yield return null;
            }

            canvasGroup.alpha = endAlpha;
        }
    }
}
