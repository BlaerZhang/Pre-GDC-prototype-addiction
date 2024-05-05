using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.VisualTools
{
    public class FloatingEffect : MonoBehaviour
    {
        public enum StartFloatingDirection
        {
            Up,
            Down
        }

        public float startingDelay;
        public float floatDuration = 2.0f;
        public float floatRange = 0.5f;
        public StartFloatingDirection startFloatingDirection;

        void Start()
        {
            StartCoroutine(Floating());
        }

        private IEnumerator Floating()
        {
            yield return new WaitForSeconds(startingDelay);

            switch (startFloatingDirection)
            {
                case StartFloatingDirection.Up:
                    floatRange = Mathf.Abs(floatRange);
                    break;
                case StartFloatingDirection.Down:
                    floatRange = -Mathf.Abs(floatRange);
                    break;
            }

            transform.DOLocalMoveY(floatRange, floatDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetRelative(true)
                .SetEase(Ease.InOutSine);
        }
    }
}