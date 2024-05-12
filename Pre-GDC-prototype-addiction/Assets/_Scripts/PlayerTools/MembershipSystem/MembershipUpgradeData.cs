using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.PlayerTools.MembershipSystem
{
    [CreateAssetMenu(fileName = "MembershipUpgradeSetting", menuName = "ScriptableObjects/MembershipUpgradeSetting", order = 0)]
    public class MembershipUpgradeData : SerializedScriptableObject
    {
        [DictionaryDrawerSettings(KeyLabel = "Current Membership Level", ValueLabel = "Points to Next Level")]
        public Dictionary<int, int> LevelUpgradePointsDict  = new();
    }
}