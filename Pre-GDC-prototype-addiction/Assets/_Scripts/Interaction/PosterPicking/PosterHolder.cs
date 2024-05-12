using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interaction
{
    public class PosterHolder : ScrollView
    {
        [Title("Poster Holder Settings")]
        [SerializeField] private float posterHolderInitialY;
        [SerializeField] private Transform posterHolderScrollIndicator;

        [Title("Poster Picking Settings")]
        [InfoBox("The vector2 is the left-top point of the rect")]
        [SerializeField] private Rect pickingArea;

        [Title("Movement Settings")]
        [SerializeField] private float moveSpeed;
        [Space]
        [InfoBox("Below are the local positions of the scroll view")]
        [SerializeField] private float scrollSemiCollapsedY;
        [SerializeField] private float scrollSemiCollapseDuration;
        [Space]
        [SerializeField] private float scrollCollapsedY;
        [SerializeField] private float scrollCollapseDuration;
        [Space]
        [SerializeField] private float fullyHideY;

        private float recoverY;

        public static Action onReturnPosterMenu;
        public static Action onRecoverHolderPosition;

        private void Start()
        {
            transform.position = new Vector3(transform.position.x, posterHolderInitialY, transform.position.z);
        }

        private void OnEnable()
        {
            ScratchCardPoster.onPosterDragged += SemiCollapse;
            ScratchCardPoster.onPosterReleased += CheckIfPosterInPickingArea;
            ScratchCardPoster.onTryBuyPoster += SwitchCollapseState;

            ScratchCardDealer.onPrizeRedeemed += RecoverPosition;
            // ScrollViewHandle.onScrollViewHandleClicked += RecoverPosition;
            // SelectableScratchCard.onScratchCardSelected += FullyHidePosterHolder;
        }

        private void OnDisable()
        {
            ScratchCardPoster.onPosterDragged -= SemiCollapse;
            ScratchCardPoster.onPosterReleased -= CheckIfPosterInPickingArea;
            ScratchCardPoster.onTryBuyPoster -= SwitchCollapseState;

            ScratchCardDealer.onPrizeRedeemed += RecoverPosition;
            // ScrollViewHandle.onScrollViewHandleClicked -= RecoverPosition;
            // SelectableScratchCard.onScratchCardSelected -= FullyHidePosterHolder;
        }

        private void SwitchCollapseState(ScratchCardPoster poster, bool isBought)
        {
            print(isBought);
            if (isBought) Collapse();
            else RecoverPosition();
        }

        private void SemiCollapse()
        {
            isScrollLocked = true;
            recoverY = scrollViewHolder.position.y;
            posterHolderScrollIndicator.SetParent(scrollViewHolder); //set parent to follow collapse
            scrollViewHolder.DOLocalMoveY(scrollSemiCollapsedY, scrollSemiCollapseDuration).SetEase(Ease.OutBack);
        }

        // called if the poster is chosen
        private void Collapse()
        {
            isScrollLocked = true;
            // recoverY = scrollViewHolder.position.y;
            posterHolderScrollIndicator.SetParent(scrollViewHolder); //set parent to follow collapse
            scrollViewHolder.DOLocalMoveY(scrollCollapsedY, scrollCollapseDuration).SetEase(Ease.OutBack);
        }

        // private void FullyHidePosterHolder(SelectableScratchCard selectableScratchCard, FaceEventType faceEventType)
        // {
        //     isScrollLocked = true;
        //     scrollViewHolder.DOLocalMoveY(fullyHideY, 0.1f).SetEase(Ease.OutCubic);
        // }

        // calculate relative position of scroll and poster holder
        private void RecoverPosition()
        {
            float duration = CalculateMovementDuration(recoverY);
            scrollViewHolder.DOLocalMoveY(recoverY, duration)
                .SetEase(Ease.OutCubic)
                .OnStart(() =>
                {
                    ScratchCardPoster.isInteractable = false;
                })
                .OnComplete(() =>
                {
                    isScrollLocked = false;
                    posterHolderScrollIndicator.SetParent(this.transform); //set parent to unfollow collapse
                    ScratchCardPoster.isInteractable = true;
                    onRecoverHolderPosition?.Invoke();
                });
        }
        

        private float CalculateMovementDuration(float targetY)
        {
            float distance = Mathf.Abs(scrollViewHolder.position.y - targetY);
            return distance / moveSpeed;
        }

        private bool CheckIfPosterInPickingArea(Vector2 posterDropPosition)
        {
            if (pickingArea.Contains(posterDropPosition)) return true;

            RecoverPosition();
            return false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(pickingArea.position + pickingArea.size / 2, pickingArea.size);
        }
    }
}