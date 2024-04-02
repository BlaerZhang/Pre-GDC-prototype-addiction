using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class UIManager : MonoBehaviour
    {
        [Header("Game")]
        public List<TextMeshProUGUI> playerResource;
    
        [Header("Incremental")]
        public TextMeshProUGUI upgradePrice;

        [Header("Buy")] 
        public TextMeshProUGUI buyCardPrice;

        private bool isPlayingNotEnoughAnimation = false;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
        }
    
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void UpdateResource(int resource)
        {
            foreach (var resourceText in playerResource)
            {
                resourceText.text = $"${resource}";
            }
        }

        public void UpdateUpgradePrice(int price)
        {
            upgradePrice.text = $"${price}";
        }

        public void UpdateBuyPrice(int price)
        { 
            print(buyCardPrice);
            buyCardPrice.text = $"${price}";
            print("Price Updated!");
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
    
    }
}
