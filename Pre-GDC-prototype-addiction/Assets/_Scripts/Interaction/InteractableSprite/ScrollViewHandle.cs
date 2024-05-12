using System;
using UnityEngine;

namespace _Scripts.Interaction.InteractableSprite
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ScrollViewHandle : InteractableSpriteBase
    {
        public static Action onScrollViewHandleClicked;

        protected override void OnMouseUp()
        {
            base.OnMouseUp();
            onScrollViewHandleClicked?.Invoke();
        }
    }
}