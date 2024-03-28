using System;
using System.Collections;
using System.Collections.Generic;
using Interaction;
using TMPro;
using UnityEngine;

public class NumberRoller : MonoBehaviour
{
    public TMP_Text numberText;
    public float duration = 2f;
    private CanvasGroup canvasGroup;

    public static Action onRollingEnds;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        PrizeRevealing.onPrizeRevealed += StartRolling;
    }

    private void OnDisable()
    {
        PrizeRevealing.onPrizeRevealed -= StartRolling;
    }

    private void StartRolling(float prize)
    {
        StartCoroutine(FadeCanvasGroupAlpha(0, 1, 1f));
        StartCoroutine(RollNumber(prize));
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

        onRollingEnds();
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
