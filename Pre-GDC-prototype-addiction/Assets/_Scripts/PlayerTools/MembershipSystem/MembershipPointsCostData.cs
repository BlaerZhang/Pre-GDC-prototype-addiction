using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.PlayerTools.MembershipSystem
{
    [CreateAssetMenu(fileName = "MembershipPointsCostSetting", menuName = "ScriptableObjects/MembershipPointsCostSetting", order = 0)]
    public class MembershipPointsCostData : SerializedScriptableObject
    {
        [DictionaryDrawerSettings(KeyLabel = "Card Price", ValueLabel = "Membership Points")]
        public Dictionary<int, int> PriceMembershipPointsDict = new();
    }
}