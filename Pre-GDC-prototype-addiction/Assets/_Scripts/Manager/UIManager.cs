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
        [Title("Game")]
        public List<TextMeshProUGUI> playerResource;

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

        private bool isPlayingNotEnoughAnimation = false;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
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

        public void UpdateResource(int resource)
        {
            foreach (var resourceText in playerResource)
            {
                if (resourceText.text == $"${resource}") return;
                resourceText.DOText($"${resource}", 1f,true, ScrambleMode.Numerals);
            }
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
            membershipLevelUI.text = currentLevel.ToString();
        }

        public Tween UpdateMembershipProgressUI(int toLevel, float targetValue, bool isResetRequired = false)
        {
            // TODO: change points left
            return membershipProgressBar.DOValue(targetValue, 0.5f).SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
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
            foreach (var resourceText in playerResource)
            {
                resourceText.DOColor(Color.red, 0.5f).SetEase(Ease.Flash, 4, 0);
                resourceText.rectTransform.DOShakeAnchorPos(0.5f, Vector3.right * 10f, 10).OnComplete((() =>
                {
                    isPlayingNotEnoughAnimation = false;
                }));
            }
        }

        private void PopFaceEventUI(FaceEventType eventType, int eventDuration, ScratchCardTier triggerTier)
        {
            if (eventType == FaceEventType.NoEvent) return;
            faceEventUIDict[eventType].DOAnchorPosX(0, 0.5f).SetEase(Ease.OutElastic);
        }
        
        private void CollapseFaceEventUI(FaceEventType eventType)
        {
            if (eventType == FaceEventType.NoEvent) return;
            faceEventUIDict[eventType].DOAnchorPosX(400, 0.3f);
        }

        private void HideFaceEventUI()
        {
            faceEventUIParent.DOAnchorPosX(400, 0.3f);
        }

        private void ShowFaceEventUI(string toScene)
        {
            faceEventUIParent.DOAnchorPosX(0, 0.3f);
        }
    }
}
