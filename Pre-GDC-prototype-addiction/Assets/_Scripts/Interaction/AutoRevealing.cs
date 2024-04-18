using DG.Tweening;
using UnityEngine;

namespace Interaction
{
    public class AutoRevealing : ScratchProgressEvent
    {
        public float fullyScratchedThreshold = 0.7f;
        public float autoRevealingDuration = 0.5f;

        private bool isFullyScratched = false;

        protected override void OnScratchProgress(float progress)
        {
            if (progress >= fullyScratchedThreshold)
            {
                if (!isFullyScratched)
                {
                    isFullyScratched = true;
                    cardManager.SpriteRendererCard?.DOFade(0, autoRevealingDuration)
                        .OnComplete(() => { cardManager.Card.Fill(); });
                }

                base.OnScratchProgress(progress);
            }
        }
    }
}