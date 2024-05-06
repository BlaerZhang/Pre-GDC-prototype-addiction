using System;
using UnityEngine;

namespace Interaction
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class HandInArea : MonoBehaviour
    {
        [SerializeField] private Vector2 areaOffset;

        protected SpriteRenderer spriteRenderer;
        protected Rect handInArea;

        private void Start()
        {
            SetHandInArea();
        }

        private void SetHandInArea()
        {
            handInArea = spriteRenderer.sprite.rect;
            handInArea.position += areaOffset;
        }

        public bool CheckIfPositionInArea(Vector2 objectPosition)
        {
            if (handInArea.Contains(objectPosition)) return true;
            return false;
        }
    }
}