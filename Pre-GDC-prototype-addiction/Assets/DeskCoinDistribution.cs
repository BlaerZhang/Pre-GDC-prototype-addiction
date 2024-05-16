using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Manager;
using _Scripts.MetaphysicsSystem;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class DeskCoinDistribution : SerializedMonoBehaviour
{
    private int currentLevel = -1;
    [Serializable]
    public class CoinUnlockInfo
    {
        public int unlockGold;
        public GameObject coinStack;
    }
    // level, object
    public List<CoinUnlockInfo> goldCoinLevelList = new();

    public AudioClip itemGenerateSound;

    private void Start()
    {
        if (goldCoinLevelList.Count == 0) return;
        foreach (var coinUnlockInfo in goldCoinLevelList)
        {
            coinUnlockInfo.coinStack.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GameManager.Instance.resourceManager.PlayerGold += 500;
        }
    }

    private void OnEnable()
    {
        StatsTracker.onValueChanged += ChangeCoinLevel;
    }

    private void OnDisable()
    {
        StatsTracker.onValueChanged -= ChangeCoinLevel;
    }

    private void ChangeCoinLevel(string valueName, float gold)
    {
        if (currentLevel == goldCoinLevelList.Count - 1)
        {
            StatsTracker.onValueChanged -= ChangeCoinLevel;
            return;
        }

        if (valueName.Equals("playerGold"))
        {
            if (goldCoinLevelList.Count == 0) return;
            if (gold >= goldCoinLevelList[currentLevel + 1].unlockGold)
            {
                GameManager.Instance.audioManager.PlaySound(itemGenerateSound);

                GameObject coinObject = goldCoinLevelList[currentLevel + 1].coinStack;

                RectTransform rectTransform = coinObject.GetComponent<RectTransform>();
                Vector2 originalPosition = rectTransform.anchoredPosition;
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + 250);
                coinObject.SetActive(true);

                GameManager.Instance.audioManager.PlaySound(itemGenerateSound);
                rectTransform.DOAnchorPosY(originalPosition.y, 0.5f).SetEase(Ease.OutBounce);

                currentLevel++;
            }
        }
    }
}
