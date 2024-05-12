using System;
using UnityEngine;

namespace _Scripts.Interaction
{
    public class MouseEnterDetector : MonoBehaviour
    {
        public static Action<global::ScratchCardAsset.ScratchCard> onMouseEnterEvent;
        public static Action onMouseExitEvent;

        private global::ScratchCardAsset.ScratchCard _scratchCard;

        private void Start()
        {
            _scratchCard = transform.parent.transform.Find("Scratch Card").GetComponent<global::ScratchCardAsset.ScratchCard>();
        }

        private void OnMouseEnter()
        {
            onMouseEnterEvent(_scratchCard);
        }

        // private void OnMouseOver()
        // {
        //     onMouseEnterEvent(_scratchCard);
        // }

        private void OnMouseExit()
        {
            onMouseExitEvent();
        }
    }
}
