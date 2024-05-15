using _Scripts.FaceEventSystem;
using _Scripts.Interaction.InteractableSprite;
using _Scripts.Interaction.PosterPicking;
using _Scripts.Manager;
using _Scripts.PlayerTools.MembershipSystem;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.PlayerTools
{
    // TODO: unlock visual effects and sprites
    public class PlayerToolBase : MonoBehaviour, IUnlockable
    {
        [SerializeField] private bool unlockRequired = true;

        [ShowIfGroup(nameof(unlockRequired))]

        [BoxGroup("unlockRequired/Unlock Settings")]
        [SerializeField] private int unlockMembershipLevel;
        [BoxGroup("unlockRequired/Unlock Settings")]
        [SerializeField] private GameObject lockedCover, unlockObject;
        [BoxGroup("unlockRequired/Unlock Settings")]
        [SerializeField] private AudioClip revealCoverSound;


        [Title("Collapse Settings")]
        [SerializeField] private bool hideWhenScratching = false;
        [SerializeField] private HidingDirection currentHidingDirection;
        [SerializeField] private float hideOffset;
        [SerializeField] public float collapseDuration = 0.5f;
        private float semiHideOffset;

        public enum HidingDirection
        {
            Top,
            Right,
            Bottom,
            Left
        }

        private RectTransform rectTransform;
        private Vector2 originalPosition;

        private bool isUnlocked = false;
        // private bool isCollapsed = false;
        // private bool isCollapsing = false;

        private void Start()
        {
            semiHideOffset = hideOffset / 3;
            rectTransform = GetComponent<RectTransform>();
            originalPosition = rectTransform.anchoredPosition;

            if (unlockObject) unlockObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                CollapseToEdge(hideOffset);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                ExpandFromEdge();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                ShowUnlockEffect();
            }
        }

        private void OnEnable()
        {
            if (hideWhenScratching)
            {
                SelectableScratchCard.onScratchCardSelected += Collapse;
                ScratchCardDealer.onPrizeRedeemed += ExpandFromEdge;
                // ScratchCardPoster.onPosterDragged += SemiCollapse;
                // ScratchCardPoster.onTryBuyPoster += SwitchCollapseState;
            }

            if (!unlockRequired) return;
            MembershipManager.onMembershipLevelUp += UnlockItem;
        }

        private void OnDisable()
        {
            if (hideWhenScratching)
            {
                SelectableScratchCard.onScratchCardSelected -= Collapse;
                ScratchCardDealer.onPrizeRedeemed -= ExpandFromEdge;
                // ScratchCardPoster.onPosterDragged -= SemiCollapse;
                // ScratchCardPoster.onTryBuyPoster -= SwitchCollapseState;
            }

            if (!unlockRequired) return;
            MembershipManager.onMembershipLevelUp -= UnlockItem;
        }

        #region Unlock Tool
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

        protected virtual void ShowUnlockEffect()
        {
            // TODO: unlock effect
            if (!lockedCover) return;

            RectTransform mainCover = lockedCover.transform.GetChild(0).GetComponent<RectTransform>();
            RectTransform coverTop = lockedCover.transform.GetChild(1).GetComponent<RectTransform>();

            if (revealCoverSound) GameManager.Instance.audioManager.PlaySound(revealCoverSound);

            float lockedCoverSecondLiftY = coverTop.GetComponent<Image>().rectTransform.rect.height;
            float lockedCoverFirstLiftY = mainCover.GetComponent<Image>().rectTransform.rect.height + lockedCoverSecondLiftY;

            mainCover.DOAnchorPosY(mainCover.anchoredPosition.y + lockedCoverFirstLiftY, 1f).SetEase(Ease.InQuart)
                .OnComplete(() =>
                {
                    coverTop.DOAnchorPosY(coverTop.anchoredPosition.y + lockedCoverSecondLiftY, 0.5f)
                        .SetEase(Ease.InQuart)
                        .OnComplete(EnableTool);
                });
        }

        private void EnableTool()
        {
            if (unlockObject)
            {
                unlockObject.transform.localScale = Vector3.zero;
                unlockObject.SetActive(true);
                unlockObject.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutBack);
            }
        }
        #endregion

        #region Hide Tool
        // private void SemiCollapse()
        // {
        //     CollapseToEdge(semiHideOffset);
        // }

        // private void SwitchCollapseState(ScratchCardPoster poster, bool isCollapseSuccessful)
        // {
        //     if (isCollapseSuccessful) Collapse();
        //     else ExpandFromEdge();
        // }

        private void Collapse(SelectableScratchCard selectableScratchCard, FaceEventType faceEventType)
        {
            CollapseToEdge(hideOffset);
        }

        // TODO: when should it be called?
        /// <summary>
        /// hide outside its closest screen border
        /// </summary>
        private void CollapseToEdge(float currentHideOffset)
        {
            // Calculate the distances to each edge
            // float distanceToLeft = rectTransform.anchoredPosition.x + Screen.width / 2 + rectTransform.rect.width / 2;
            // float distanceToRight = Screen.width / 2 - rectTransform.anchoredPosition.x + rectTransform.rect.width / 2;
            // float distanceToTop = Screen.height / 2 - rectTransform.anchoredPosition.y + rectTransform.rect.height / 2;
            // float distanceToBottom = rectTransform.anchoredPosition.y + Screen.height / 2 + rectTransform.rect.height / 2;

            // Find the minimum distance
            // float minDistance = Mathf.Min(distanceToLeft, distanceToRight, distanceToTop, distanceToBottom);

            // Determine the closest edge and set target position accordingly
            Vector2 targetPosition = originalPosition;
            // if (Mathf.Approximately(minDistance, distanceToLeft))
            if (currentHidingDirection.Equals(HidingDirection.Left))
            {
                // targetPosition.x = -(Screen.width / 2 + rectTransform.rect.width / 2 + currentHideOffset);
                targetPosition.x = -(currentHideOffset);
            }
            // else if (Mathf.Approximately(minDistance, distanceToRight))
            else if (currentHidingDirection.Equals(HidingDirection.Right))
            {
                // targetPosition.x = Screen.width / 2 + rectTransform.rect.width / 2 + currentHideOffset;
                targetPosition.x = currentHideOffset;
            }
            // else if (Mathf.Approximately(minDistance, distanceToTop))
            else if (currentHidingDirection.Equals(HidingDirection.Top))
            {
                // targetPosition.y = Screen.height / 2 + rectTransform.rect.height / 2 + currentHideOffset;
                targetPosition.y = currentHideOffset;
            }
            // else if (Mathf.Approximately(minDistance, distanceToBottom))
            else if (currentHidingDirection.Equals(HidingDirection.Bottom))
            {
                // targetPosition.y = -(Screen.height / 2 + rectTransform.rect.height / 2 + currentHideOffset);
                targetPosition.y = -(currentHideOffset);
            }

            rectTransform.DOAnchorPos(targetPosition, collapseDuration).SetEase(Ease.InOutQuad).SetEase(Ease.OutBack);
        }

        private void ExpandFromEdge()
        {
            rectTransform.DOAnchorPos(originalPosition, collapseDuration).SetEase(Ease.InOutQuad).SetEase(Ease.OutCubic);
        }

        // /// <summary>
        // /// Hide/show tools
        // /// </summary>
        // /// <param name="targetPosition"></param>
        // private void MoveTool(Vector2 targetPosition)
        // {
        //     // if (isCollapsing) return;
        //     // TODO: GIMME SOME JUICE!!!
        //     rectTransform.DOAnchorPos(targetPosition, collapseDuration).SetEase(Ease.InOutQuad).OnStart(() =>
        //     {
        //         isCollapsing = true;
        //     }).OnComplete(() =>
        //     {
        //         isCollapsing = false;
        //         isCollapsed = !isCollapsed;
        //     }).SetEase(Ease.OutBack);
        // }
        #endregion
    }
}