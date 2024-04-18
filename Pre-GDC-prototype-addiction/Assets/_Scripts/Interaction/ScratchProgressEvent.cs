using System;
using ScratchCardAsset;
using UnityEngine;

namespace Interaction
{
    public abstract class ScratchProgressEvent : MonoBehaviour
    {
        protected ScratchCardManager cardManager;

        private void Start()
        {
            cardManager = GetComponent<ScratchCardManager>();
            cardManager.Progress.OnProgress += OnScratchProgress;
        }

        protected virtual void OnScratchProgress(float progress)
        {
            cardManager.Progress.OnProgress -= OnScratchProgress;
        }
    }
}