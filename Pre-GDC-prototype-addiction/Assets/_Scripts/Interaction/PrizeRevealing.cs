using System;
using System.Collections;
using System.Globalization;
using Manager;
using ScratchCardAsset;
using ScratchCardGeneration;
using ScratchCardGeneration.LayoutConstructor;
using ScratchCardGeneration.Utilities;
using TMPro;
using UnityEngine;

namespace Interaction
{
    /// <summary>
    /// on winning prize icon
    /// </summary>
    public class PrizeRevealing : ScratchProgressEvent
    {
        [HideInInspector] public bool isWinningPrize = false;

        [HideInInspector] public Vector2Int currentGrid;

        public float fullyScratchedThreshold = 0.7f;
        private bool isFullyScratched = false;

        private bool isClickable = false;
        private bool hasClicked = false;
        private bool hasNumberShown = false;
        [HideInInspector] public float prize;

        private bool hasSubmitted = false;

        // pass the current fully scratched grid
        public static Action<Vector2Int> onFullyScratched;
        public static Action<float> onPrizeRevealed;

        // private ScratchCardManager cardManager;

        private void Update()
        {
            if (isWinningPrize) ClickIcon();
        }

        private void OnEnable()
        {
            NumberRoller.onRollingEnds += GeneratePrizeNumber;
            if (!isWinningPrize) BuyCardManager.onChangeSubmissionStatus += ChangeSubmissionStatus;
        }

        private void OnDisable()
        {
            NumberRoller.onRollingEnds -= GeneratePrizeNumber;
            if (!isWinningPrize) BuyCardManager.onChangeSubmissionStatus -= ChangeSubmissionStatus;
        }

        private void ChangeSubmissionStatus()
        {
            // print("changing status");
            hasSubmitted = true;
        }

        protected override void OnScratchProgress(float progress)
        {
            if (progress >= fullyScratchedThreshold)
            {
                if (!isFullyScratched)
                {
                    isFullyScratched = true;
                    FruitiesLayoutConstructor fruitiesLayoutConstructor =
                        (FruitiesLayoutConstructor)GameManager.Instance.scratchCardGenerator.CardLayoutConstructorDic[
                            ScratchCardBrand.Fruities];
                    fruitiesLayoutConstructor.ScratchingStatusMatrix.SetElement(currentGrid.x, currentGrid.y, true);

                    // print("fully scratched, clear the grid");
                    // cardManager.Card.ScratchHole(new Vector2(0f, 0f));
                    onFullyScratched?.Invoke(currentGrid);
                }
                
                cardManager.Progress.OnProgress -= OnScratchProgress;
                if (!isWinningPrize) return;
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
                onPrizeRevealed?.Invoke(prize);
            }
        }

        private void GeneratePrizeNumber(TMP_FontAsset fontAsset)
        {
            GameObject textObject;
            TextMeshPro textMeshPro;
            if (isWinningPrize)
            {
                // it must be revealed, and must be clicked
                if (hasNumberShown || !hasClicked) return;

                textObject = new GameObject("prize")
                {
                    transform =
                    {
                        localRotation = Quaternion.Euler(0, 0, 30f)
                    }
                };
                textMeshPro = textObject.AddComponent<TextMeshPro>();
                textObject.GetComponent<RectTransform>().sizeDelta = Vector2.one;
                textMeshPro.text = prize.ToString(CultureInfo.InvariantCulture);
                textMeshPro.fontStyle = FontStyles.Bold;
                textMeshPro.color = new Color(1f, 195f/255f, 0f, 1f);
                textMeshPro.fontSize = 10;
                hasNumberShown = true;
            }
            else
            {
                if (!hasSubmitted) return;

                textObject = new GameObject("fake prize")
                {
                    transform =
                    {
                        localRotation = Quaternion.Euler(0, 0, 30f)
                    }
                };
                textMeshPro = textObject.AddComponent<TextMeshPro>();
                textObject.GetComponent<RectTransform>().sizeDelta = Vector2.one;
                textMeshPro.text = Utils.GenerateCleanNumber(1, 3).ToString();
                textMeshPro.fontStyle = FontStyles.Italic;
                textMeshPro.color = Color.white;
                textMeshPro.fontSize = 5;
            }

            textMeshPro.enableWordWrapping = false;
            textMeshPro.font = fontAsset;
            textMeshPro.alignment = TextAlignmentOptions.Center;
            textMeshPro.sortingOrder = 2;
            textObject.transform.SetParent(transform);
            textObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, -0.5f);
        }
    }
}
