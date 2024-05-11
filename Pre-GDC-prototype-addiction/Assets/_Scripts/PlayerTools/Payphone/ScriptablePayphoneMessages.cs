using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.PlayerTools.Payphone
{
    [CreateAssetMenu(fileName = "ScriptablePayphoneMessages", menuName = "ScriptableObjects/ScriptablePayphoneMessages", order = 0)]
    public class ScriptablePayphoneMessages : SerializedScriptableObject
    {
        [InfoBox("Message name is used to identify messages, message is a list, each element in the list will be shown individually in a text bubble")]
        [DictionaryDrawerSettings(KeyLabel = "Message Name", ValueLabel = "Message Text", DisplayMode = DictionaryDisplayOptions.Foldout)]
        public Dictionary<string, List<string>> PhoneMessageDict = new();
    }
}