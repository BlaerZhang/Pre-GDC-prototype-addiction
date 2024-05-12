using System;
using System.Collections.Generic;
using _Scripts.CustomEventSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.MetaphysicsSystem
{
    /// <summary>
    /// single metaphysics data
    /// </summary>
    [CreateAssetMenu(fileName = "NewScriptableMetaphysics", menuName = "ScriptableObjects/ScriptableMetaphysics", order = 0)]
    public class ScriptableMetaphysics : SerializedScriptableObject
    {
        [Serializable]
        public class Requirement
        {
            public Comparator comparator;
            public float value;
            [HideInInspector] public bool isMatched = false;
        }

        public int metaphysicsId;
        [DictionaryDrawerSettings(KeyLabel = "Variable Name", ValueLabel = "Requirements")]
        public Dictionary<string, Requirement> metaphysicsRequirement = new Dictionary<string, Requirement>();
        public GameEvent response;
        public bool isRepeatable = true;
        [HideInInspector] public bool hasTriggered;

        public void ResetData()
        {
            hasTriggered = false;
        }
    }

    public enum Comparator
    {
        Equal,
        Large,
        Smaller,
        LargeEqual,
        SmallerEqual
    }
}