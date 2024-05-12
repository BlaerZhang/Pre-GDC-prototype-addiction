using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Manager
{
    public class UIManager : SerializedMonoBehaviour
    {
        [Title("Resource")]
        public TextMeshProUGUI goldUI;

        [Title("Face Event")]
        public RectTransform faceEventUIParent;
        public Dictionary<FaceEventType, RectTransform> faceEventUIDict;
    
        [Title("Incremental")]
        public TextMeshProUGUI upgradePrice;

        [Title("Buy")]
        public TextMeshProUGUI buyCardPrice;

        [Title("Membership Card UI")]
        [SerializeField] private Slider membershipProgressBar;
        [SerializeField] private TextMeshProUGUI membershipLevelUI;
        public float progressBarZoomRatio = 1.3f;

        private bool isPlayingNotEnoughAnimation = false;

        private void OnEnable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            FaceEventListener.onFaceEventTriggered += PopFaceEventUI;
            FaceEventManager.onFaceEventEnd += CollapseFaceEventUI;
            SwitchSceneManager.onSceneChanged += ShowFaceEventUI;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            FaceEventListener.onFaceEventTriggered -= PopFaceEventUI;
            FaceEventManager.onFaceEventEnd -= CollapseFaceEventUI;
            SwitchSceneManager.onSceneChanged -= ShowFaceEventUI;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            switch (scene.name)
            {
                case "Incremental":
                    upgradePrice = GameObject.Find("Upgrade Price").GetComponent<TextMeshProUGUI>();
                    break;
                case "Buy Card":
                    buyCardPrice = GameObject.Find("Buy Price").GetComponent<TextMeshProUGUI>();
                    break;
            }
            
            HideFaceEventUI();
        }

        public void UpdateGold(int resource)
        {
            if (!goldUI) return;
            if (goldUI.text == $"{resource}") return;
            goldUI.DOText($"{resource}", 1f,true, ScrambleMode.Numerals);
        }

        public void UpdateUpgradePrice(int price)
        {
            upgradePrice.text = $"${price}";
        }

        public void UpdateBuyPrice(int price)
        { 
            buyCardPrice.text = $"${price}";
        }

        private void UpdateMembershipLevel(int currentLevel)
        {
            Sequence levelUpSequence = DOTween.Sequence();
            levelUpSequence
                .Append(membershipLevelUI.rectTransform.DOScale(1.2f, 0.25f))
                .Append(membershipLevelUI.rectTransform.DOLocalRotate(new Vector3(90, 0, 0), 0.25f))
                .Append(membershipLevelUI.DOText(currentLevel.ToString(), 0))
                .Append(membershipLevelUI.rectTransform.DOLocalRotate(new Vector3(0, 0, 0), 0.25f))
                .Append(membershipLevelUI.rectTransform.DOScale(1f, 0.25f));
            levelUpSequence.Play();
        }

        public Tween UpdateMembershipProgressUI(int toLevel, float targetValue, bool isResetRequired = false)
        {
            // TODO: change points left
            return membershipProgressBar.DOValue(targetValue, 0.5f).SetEase(Ease.OutCubic)
                .OnStart(() =>
                {
                    membershipProgressBar.transform.DOScale(progressBarZoomRatio, 0.25f).SetEase(Ease.OutCubic);
                })
                .OnComplete(() =>
                {
                    membershipProgressBar.transform.DOScale(1f, 0.25f).SetEase(Ease.InCubic);
                    if (isResetRequired)
                    {
                        ShowMembershipUpgradeEffect();
                        UpdateMembershipLevel(toLevel);
                        membershipProgressBar.value = 0;
                    }
                });
        }

        private void ShowMembershipUpgradeEffect()
        {
            // TODO:
        }
    
        public void PlayNotEnoughGoldAnimation()
        {
            if(isPlayingNotEnoughAnimation) return;
            isPlayingNotEnoughAnimation = true;

            goldUI.DOColor(Color.red, 0.5f).SetEase(Ease.Flash, 4, 0);
            goldUI.rectTransform.DOShakeAnchorPos(0.5f, Vector3.right * 10f, 10).OnComplete((() =>
            {
                isPlayingNotEnoughAnimation = false;
            }));

        }

        private void PopFaceEventUI(FaceEventType eventType, int eventDuration, ScratchCardTier triggerTier)
        {
            if (eventType == FaceEventType.NoEvent) return;
            faceEventUIDict[eventType].DOAnchorPosX(0, 0.5f).SetEase(Ease.OutElastic);
        }
        
        private void CollapseFaceEventUI(FaceEventType eventType)
        {
            if (eventType == FaceEventType.NoEvent) return;
            faceEventUIDict[eventType].DOAnchorPosX(600, 0.3f);
        }

        private void HideFaceEventUI()
        {
            faceEventUIParent.DOAnchorPosX(600, 0.3f);
        }

        private void ShowFaceEventUI(string toScene)
        {
            faceEventUIParent.DOAnchorPosX(0, 0.3f);
        }
    }
}
