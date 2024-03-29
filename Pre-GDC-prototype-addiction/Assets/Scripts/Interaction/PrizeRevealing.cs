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
        private bool hasNumberShown = false;
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
            // it must be clickable(revealed) and has not been clicked
            if (!isClickable || hasClicked) return;
            // if the scratch field is scratched off
            print("rolling number!");
            onPrizeRevealed(prize);
            hasClicked = true;
        }

        private void GeneratePrizeNumber(TMP_FontAsset fontAsset)
        {
            // it must be revealed, and must be clicked
            if (hasNumberShown) return;

            GameObject textObject = new GameObject("prize");
            textObject.transform.localRotation = Quaternion.Euler(0, 0, 30f);
            TextMeshPro textMeshPro = textObject.AddComponent<TextMeshPro>();
            textObject.GetComponent<RectTransform>().sizeDelta = Vector2.one;
            textMeshPro.text = prize.ToString();
            textMeshPro.enableWordWrapping = false;
            textMeshPro.font = fontAsset;
            textMeshPro.fontStyle = FontStyles.Bold;
            textMeshPro.color = new Color(1f, 195f/255f, 0f, 1f);
            textMeshPro.fontSize = 10;
            textMeshPro.alignment = TextAlignmentOptions.Center;
            textObject.transform.SetParent(transform);
            textObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, -0.5f);

            hasNumberShown = true;
        }
    }
}
