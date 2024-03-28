using System;
using System.Collections;
using ScratchCardAsset;
using UnityEngine;

namespace Interaction
{
    /// <summary>
    /// on winning prize icon
    /// </summary>
    public class PrizeRevealing : MonoBehaviour
    {
        private bool hasRevealed = false;
        private bool isClickable = false;
        public float prize;

        public static Action<float> onPrizeRevealed;

        private ScratchCardManager cardManager;

        private void Start()
        {
            cardManager = GetComponentInChildren<ScratchCardManager>();
            cardManager.Progress.OnProgress += OnScratchProgress;
        }

        private void OnScratchProgress(float progress)
        {
            if (progress >= 0.9f)
            {
                cardManager.Progress.OnProgress -= OnScratchProgress;
                isClickable = true;
                Debug.Log($"User scratched {Math.Round(progress * 100f, 2)}% of surface");
            }
        }

        private void OnMouseDown()
        {
            if (hasRevealed || !isClickable) return;
            // if the scratch field is scratched off
            print("rolling number!");
            onPrizeRevealed(prize);
            hasRevealed = true;
        }
    }
}
