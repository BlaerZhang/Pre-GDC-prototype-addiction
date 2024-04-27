using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.PlayerTools
{
    public class PlayerToolBase : MonoBehaviour, IUnlockable
    {
        [Title("Unlock Settings")]
        [SerializeField] private bool unlockRequired = true;
        [ShowIf(nameof(unlockRequired))]
        [SerializeField] private int unlockPrice;
        [ShowIf(nameof(unlockRequired))]
        [SerializeField] private int unlockMembershipLevel;

        [Title("Collapse Settings")]
        [SerializeField] private float hideOffset;
        [SerializeField] private float collapseDuration = 0.5f;

        private RectTransform rectTransform;
        private Vector2 originalPosition;

        private bool isUnlocked = false;
        private bool isCollapsed = false;
        private bool isCollapsing = false;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            originalPosition = rectTransform.anchoredPosition;
        }

        private void OnEnable()
        {
            if (!unlockRequired) return;
            MembershipManager.onMembershipLevelUp += UnlockItem;
        }

        private void OnDisable()
        {
            if (!unlockRequired) return;
            MembershipManager.onMembershipLevelUp -= UnlockItem;
        }

        /// <summary>
        /// called when the membership levels up
        /// </summary>
        /// <param name="currentLevel"></param>
        public virtual void UnlockItem(int currentLevel)
        {
            if (!unlockRequired) return;
            if (currentLevel >= unlockMembershipLevel)
            {
                if (!isUnlocked)
                {
                    print($"Player tool {gameObject.name} unlocked");
                    ShowUnlockEffect();
                    isUnlocked = true;
                }
            }
        }

        protected virtual void ShowUnlockEffect() {}

        // private void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.A))
        //     {
        //         CollapseToEdge();
        //     }
        //
        //     if (Input.GetKeyDown(KeyCode.S))
        //     {
        //         ExpandFromEdge();
        //     }
        // }

        // TODO: when should it be called?
        /// <summary>
        /// hide outside its closest screen border
        /// </summary>
        private void CollapseToEdge()
        {
            // Calculate the distances to each edge
            float distanceToLeft = rectTransform.anchoredPosition.x + Screen.width / 2 + rectTransform.rect.width / 2;
            float distanceToRight = Screen.width / 2 - rectTransform.anchoredPosition.x + rectTransform.rect.width / 2;
            float distanceToTop = Screen.height / 2 - rectTransform.anchoredPosition.y + rectTransform.rect.height / 2;
            float distanceToBottom = rectTransform.anchoredPosition.y + Screen.height / 2 + rectTransform.rect.height / 2;

            // Find the minimum distance
            float minDistance = Mathf.Min(distanceToLeft, distanceToRight, distanceToTop, distanceToBottom);

            // Determine the closest edge and set target position accordingly
            Vector2 targetPosition = originalPosition;
            if (Mathf.Approximately(minDistance, distanceToLeft))
            {
                targetPosition.x = -(Screen.width / 2 + rectTransform.rect.width / 2 + hideOffset);
            }
            else if (Mathf.Approximately(minDistance, distanceToRight))
            {
                targetPosition.x = Screen.width / 2 + rectTransform.rect.width / 2 + hideOffset;
            }
            else if (Mathf.Approximately(minDistance, distanceToTop))
            {
                targetPosition.y = Screen.height / 2 + rectTransform.rect.height / 2 + hideOffset;
            }
            else if (Mathf.Approximately(minDistance, distanceToBottom))
            {
                targetPosition.y = -(Screen.height / 2 + rectTransform.rect.height / 2 + hideOffset);
            }

            // Use DOTween to animate the panel to the target position
            MoveTool(targetPosition);
        }

        private void ExpandFromEdge()
        {
            // Animate back to the original position
            MoveTool(originalPosition);
        }

        private void MoveTool(Vector2 targetPosition)
        {
            if (isCollapsing) return;
            // TODO: GIMME SOME JUICE!!!
            rectTransform.DOAnchorPos(targetPosition, collapseDuration).SetEase(Ease.InOutQuad).OnStart(() =>
            {
                isCollapsing = true;
            }).OnComplete(() =>
            {
                isCollapsing = false;
                isCollapsed = !isCollapsed;
            });
        }
    }
}