using System;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.VisualTools
{
    public class SpriteScaling : MonoBehaviour
    {
        public float scaleDuration;
        public float maxScale;
        private void Start()
        {
            transform.DOScale(maxScale, scaleDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }
}