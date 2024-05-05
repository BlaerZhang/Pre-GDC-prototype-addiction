using System;
using ScratchCardAsset;
using UnityEngine;

namespace Interaction
{
    public class MouseEnterDetector : MonoBehaviour
    {
        public static Action<ScratchCardAsset.ScratchCard> onMouseEnterEvent;
        public static Action onMouseExitEvent;

        private ScratchCardAsset.ScratchCard _scratchCard;

        private void Start()
        {
            _scratchCard = transform.parent.transform.Find("Scratch Card").GetComponent<ScratchCardAsset.ScratchCard>();
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
