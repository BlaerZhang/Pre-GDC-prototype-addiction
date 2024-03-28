using System;
using ScratchCardAsset;
using UnityEngine;

namespace Interaction
{
    public class MouseEnterDetector : MonoBehaviour
    {
        public static Action<ScratchCard> onMouseEnterEvent;
        public static Action onMouseExitEvent;

        private ScratchCard _scratchCard;

        private void Start()
        {
            _scratchCard = transform.parent.transform.Find("Scratch Card").GetComponent<ScratchCard>();
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
