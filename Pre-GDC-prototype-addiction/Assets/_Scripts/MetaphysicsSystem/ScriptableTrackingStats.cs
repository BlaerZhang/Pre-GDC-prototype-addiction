using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace MetaphysicsSystem
{
    /// <summary>
    /// put all data that needs to be tracked here
    /// </summary>
    [CreateAssetMenu(fileName = "TrackingStats", menuName = "ScriptableObjects/TrackingStats", order = 0)]
    public class ScriptableTrackingStats : SerializedScriptableObject
    {
        [DictionaryDrawerSettings(KeyLabel = "Name", ValueLabel = "Value")]
        public Dictionary<string, float> nameValuePair;
    }
}