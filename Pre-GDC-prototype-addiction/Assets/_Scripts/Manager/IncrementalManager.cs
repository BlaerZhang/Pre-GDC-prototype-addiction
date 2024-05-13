using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Scripts.Manager
{
    public class IncrementalManager : MonoBehaviour
    {
        // public int clickerLevel = 1;

        public List<int> productivityPerLevel;

        public List<int> upgradePricePerLevel;

        public RectMask2D incrementalUIMask;

        private ResourceManager resourceManager;

        [Header("Feedback")]
        public bool feedback;
        public GameObject textFeedback;
        public List<AudioClip> soundFeedbacks;
    
        [Header("Move to Menu")] public RectTransform menuButton;
    
        private void OnEnable()
        {

        }

        private void OnDisable()
        {
            
        }
    
        void Start()
        {
            resourceManager = GameManager.Instance.resourceManager;
            GameManager.Instance.uiManager.UpdateUpgradePrice(upgradePricePerLevel[resourceManager.ClickerLevel - 1]);
            incrementalUIMask.padding = new Vector4(0, incrementalUIMask.rectTransform.rect.height, 0, 0); // z is right, w is top, y is bottom
        }

        public void OnClick(RectTransform buttonTransform)
        {
            resourceManager.PlayerGold += productivityPerLevel[resourceManager.ClickerLevel - 1];
            resourceManager.ChangeTime(1);
            if (feedback)
            {
                PlayFeedbackAnimation(buttonTransform);
                if (soundFeedbacks.Count > 0) GameManager.Instance.audioManager.PlaySound(soundFeedbacks[Random.Range(0, soundFeedbacks.Count)]);
            }
        }

        public void OnUpgrade()
        {
            if (resourceManager.PlayerGold < upgradePricePerLevel[resourceManager.ClickerLevel - 1]) 
            {
                GameManager.Instance.uiManager.PlayNotEnoughGoldAnimation();
                return;
            } 
            resourceManager.PlayerGold -= upgradePricePerLevel[resourceManager.ClickerLevel - 1];
            resourceManager.ClickerLevel++;
            GameManager.Instance.uiManager.UpdateUpgradePrice(upgradePricePerLevel[resourceManager.ClickerLevel - 1]);
        }

        public void SwitchToIncremental()
        {
            DOTween.To(() => incrementalUIMask.padding, x => incrementalUIMask.padding = x, Vector4.zero, 1)
                .SetUpdate(true)
                .OnStart(() =>
                {
                    incrementalUIMask.padding = new Vector4(0, incrementalUIMask.rectTransform.rect.height, 0, 0);
                    AudioListener.pause = true;
                    Time.timeScale = 0;
                })
                .OnComplete(() =>
                {
                    Time.timeScale = 1;
                    ActivateMenuButton();
                });
        }

        public void SwitchToLobby()
        {
            DOTween.To(() => incrementalUIMask.padding, x => incrementalUIMask.padding = x, new Vector4(0, incrementalUIMask.rectTransform.rect.height, 0, 0), 1)
                .SetUpdate(true)
                .OnStart(() =>
                {
                    incrementalUIMask.padding = Vector4.zero;
                    DeactivateMenuButton();
                    Time.timeScale = 0;
                })
                .OnComplete(() =>
                {
                    Time.timeScale = 1;
                    AudioListener.pause = false;
                });
        }
    
        private void PlayFeedbackAnimation(RectTransform buttonTransform)
        {
            Vector3 targetPos = buttonTransform.anchoredPosition + new Vector2(Random.Range(-50f, 50f), 200);

            GameObject feedback = Instantiate(textFeedback, buttonTransform);
            TextMeshProUGUI feedbackText = feedback.GetComponentInChildren<TextMeshProUGUI>();
            Sequence feedbackSequence = DOTween.Sequence();

            feedbackText.text = $"${productivityPerLevel[resourceManager.ClickerLevel - 1]}";
        
            feedbackSequence
                .Append(feedbackText.rectTransform.DOScale(Vector3.zero, 0))
                .Append(feedbackText.rectTransform.DOScale(Vector3.one, 0.5f))
                .Insert(0, feedbackText.rectTransform.DOAnchorPos(targetPos, 2f))
                .Insert(1, feedbackText.DOFade(0, 1f))
                .OnComplete((() => { Destroy(feedback); }));
            feedbackSequence.Play();
        }
    
        public void ActivateMenuButton()
        {
            menuButton.gameObject.SetActive(true);
            menuButton.DOAnchorPosX(-125, 0.1f);
        }

        public void DeactivateMenuButton()
        {
            menuButton.DOAnchorPosX(200, 0.1f).OnComplete((() => { menuButton.gameObject.SetActive(false); }));
        }
    }
}
