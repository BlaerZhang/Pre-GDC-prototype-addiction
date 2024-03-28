using System;
using System.Collections;
using ScratchCardAsset;
using TMPro;
using UnityEngine;

namespace Interaction
{
    /// <summary>
    /// on winning prize icon
    /// </summary>
    public class PrizeRevealing : MonoBehaviour
    {
        private bool isClickable = false;
        private bool hasClicked = false;
        public float prize;

        public static Action<float> onPrizeRevealed;

        private ScratchCardManager cardManager;

        private void Start()
        {
            cardManager = GetComponentInChildren<ScratchCardManager>();
            cardManager.Progress.OnProgress += OnScratchProgress;
        }

        private void OnEnable()
        {
            NumberRoller.onRollingEnds += GeneratePrizeNumber;
        }

        private void OnDisable()
        {
            NumberRoller.onRollingEnds -= GeneratePrizeNumber;
        }

        private void OnScratchProgress(float progress)
        {
            if (progress >= 0.999f)
            {
                cardManager.Progress.OnProgress -= OnScratchProgress;
                isClickable = true;
                Debug.Log($"User scratched {Math.Round(progress * 100f, 2)}% of surface");
            }
        }

        private void OnMouseDown()
        {
            if (!isClickable || hasClicked) return;
            // if the scratch field is scratched off
            print("rolling number!");
            onPrizeRevealed(prize);
            hasClicked = true;
        }

        private void GeneratePrizeNumber()
        {
            print("gene");
            if (!hasClicked) return;
            print("number!");

            GameObject textObject = new GameObject("prize");
            textObject.transform.localRotation = Quaternion.Euler(0, 0, 30f);
            TextMeshPro textMeshPro = textObject.AddComponent<TextMeshPro>();
            textObject.GetComponent<RectTransform>().sizeDelta = Vector2.one;
            textMeshPro.text = prize.ToString();
            textMeshPro.color = Color.green;
            textMeshPro.fontSize = 4;
            textMeshPro.alignment = TextAlignmentOptions.Center;
            textObject.transform.SetParent(transform);
            textObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, -0.5f);
        }
    }
}
