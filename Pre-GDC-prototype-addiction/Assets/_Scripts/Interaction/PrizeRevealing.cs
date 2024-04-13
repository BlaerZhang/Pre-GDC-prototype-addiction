using System;
using System.Collections;
using System.Globalization;
using ScratchCardAsset;
using ScratchCardGeneration;
using ScratchCardGeneration.Utilities;
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
            cardManager = GetComponent<ScratchCardManager>();
            cardManager.Progress.OnProgress += OnScratchProgress;
        }

        private void Update()
        {
            ClickIcon();
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

        private void ClickIcon()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButtonDown(0) && ((Vector2)transform.position - mousePosition).magnitude <= 0.5f)
            {
                // it must be clickable(revealed) and has not been clicked
                if (!isClickable || hasClicked) return;
                // if the scratch field is scratched off
                // print("rolling number!");
                hasClicked = true;
                onPrizeRevealed(prize);
            }
        }

        private void GeneratePrizeNumber(TMP_FontAsset fontAsset)
        {
            // it must be revealed, and must be clicked
            if (hasNumberShown || !hasClicked) return;

            GameObject textObject = new GameObject("prize")
            {
                transform =
                {
                    localRotation = Quaternion.Euler(0, 0, 30f)
                }
            };
            TextMeshPro textMeshPro = textObject.AddComponent<TextMeshPro>();
            textObject.GetComponent<RectTransform>().sizeDelta = Vector2.one;
            textMeshPro.text = prize.ToString(CultureInfo.InvariantCulture);
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
