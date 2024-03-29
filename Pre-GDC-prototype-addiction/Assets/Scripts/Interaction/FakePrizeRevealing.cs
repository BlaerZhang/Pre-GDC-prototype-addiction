using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FakePrizeRevealing : MonoBehaviour
{
    private bool hasSubmitted = false;
    private void OnEnable()
    {
        GiveCardTemp.onChangeSubmissionStatus += ChangeSubmissionStatus;
        NumberRoller.onRollingEnds += GeneratePrizeNumber;
    }

    private void OnDisable()
    {
        GiveCardTemp.onChangeSubmissionStatus -= ChangeSubmissionStatus;
        NumberRoller.onRollingEnds -= GeneratePrizeNumber;
    }

    private void ChangeSubmissionStatus()
    {
        print("changing status");
        hasSubmitted = true;
    }

    private void GeneratePrizeNumber(TMP_FontAsset fontAsset)
    {
        print(hasSubmitted);
        if (!hasSubmitted) return;

        GameObject textObject = new GameObject("prize");
        textObject.transform.localRotation = Quaternion.Euler(0, 0, 30f);
        TextMeshPro textMeshPro = textObject.AddComponent<TextMeshPro>();
        textObject.GetComponent<RectTransform>().sizeDelta = Vector2.one;
        textMeshPro.text = Utils.GenerateCleanNumber(1000).ToString();
        textMeshPro.enableWordWrapping = false;
        textMeshPro.font = fontAsset;
        textMeshPro.fontStyle = FontStyles.Bold;
        textMeshPro.color = Color.red;
        textMeshPro.fontSize = 5;
        textMeshPro.alignment = TextAlignmentOptions.Center;
        textObject.transform.SetParent(transform);
        textObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, -0.5f);
    }
}
