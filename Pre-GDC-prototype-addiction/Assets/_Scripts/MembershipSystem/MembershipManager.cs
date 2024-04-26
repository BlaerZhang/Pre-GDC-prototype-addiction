using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.MembershipSystem;
using DG.Tweening;
using Manager;
using ScratchCardGeneration.LayoutConstructor;
using ScratchCardGeneration.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

public class MembershipManager : MonoBehaviour
{
    // scriptable object for membership upgrade
    [SerializeField] private MembershipPointsCostData membershipPointsCostData;
    // [SerializeField] private MembershipUpgradeData membershipUpgradeData;

    [SerializeField] private float basePointsToNextLevel;

    private Dictionary<int, int> priceMembershipPointsDict = new();
    // private Dictionary<int, int> levelUpgradePointsDict = new();

    private int membershipLevel = 0;

    private int totalPointsToNextLevel;
    private int currentPointsToNextLevel;

    public static Action<int> onMembershipLevelUp;

    private void Start()
    {
        CreateData();

        totalPointsToNextLevel = (int)CalculateTotalPointsToNextLevel();
        currentPointsToNextLevel = totalPointsToNextLevel;
        // print("totalPointsToNextLevel" + totalPointsToNextLevel);
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.A))
    //     {
    //         GainMembershipPoints(10000);
    //         // print("currentPointsToNextLevel" + currentPointsToNextLevel);
    //     }
    // }

    private void CreateData()
    {
        priceMembershipPointsDict = Utils.DeepCopyDictionary(membershipPointsCostData.PriceMembershipPointsDict);
        // levelUpgradePointsDict = Utils.DeepCopyDictionary(membershipUpgradeData.LevelUpgradePointsDict);
    }

    /// <summary>
    /// called after card (or other consumable) is bought
    /// </summary>
    /// <param name="itemPrice"></param>
    public void GainMembershipPoints(int itemPrice)
    {
        // transform price to points gained
        if (!priceMembershipPointsDict.TryGetValue(itemPrice, out var pointsGained)) Debug.LogError($"membershipUpgradeData miss the key {itemPrice}");

        // calculate the level upgraded
        Sequence gainPointsProgressSequence = DOTween.Sequence();

        while (pointsGained > 0)
        {
            if (currentPointsToNextLevel > pointsGained)
            {
                currentPointsToNextLevel -= pointsGained;

                float targetValue = (float)(totalPointsToNextLevel - currentPointsToNextLevel) / totalPointsToNextLevel;
                gainPointsProgressSequence.Append(GameManager.Instance.uiManager.UpdateMembershipProgressUI(membershipLevel, targetValue));

                pointsGained = 0;
            }
            else
            {
                pointsGained -= currentPointsToNextLevel;
                currentPointsToNextLevel = 0;

                // level up if exceed the value of points required
                UpgradeMembershipLevel();

                float targetValue = (float)(totalPointsToNextLevel - currentPointsToNextLevel) / totalPointsToNextLevel;
                gainPointsProgressSequence.Append(GameManager.Instance.uiManager.UpdateMembershipProgressUI(membershipLevel, targetValue, true));

                // totalPointsToNextLevel = levelUpgradePointsDict[membershipLevel];
                totalPointsToNextLevel = (int)CalculateTotalPointsToNextLevel();
                // print("totalPointsToNextLevel" + totalPointsToNextLevel);
                currentPointsToNextLevel = totalPointsToNextLevel;
            }
        }

        gainPointsProgressSequence.Play();
    }

    private float CalculateTotalPointsToNextLevel()
    {
        return 2.5f * Mathf.Pow(membershipLevel, 2) + 2.5f * membershipLevel + basePointsToNextLevel;
    }

    private void UpgradeMembershipLevel()
    {
        print("membership leveling up!");
        membershipLevel++;
        onMembershipLevelUp?.Invoke(membershipLevel);
    }
}
