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
    }
}
