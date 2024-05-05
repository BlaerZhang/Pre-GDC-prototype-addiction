using System;
using UnityEngine;
using DG.Tweening;

namespace _Scripts.VisualTools
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class InfiniteScroll : MonoBehaviour
    {
        public Vector2 moveAmount;
        public float cycleDuration = 5.0f;

        void Start()
        {
            transform.DOLocalMove(moveAmount, cycleDuration)
                .SetLoops(-1, LoopType.Restart)
                .SetRelative(true)
                .SetEase(Ease.Linear);
        }
    }
}