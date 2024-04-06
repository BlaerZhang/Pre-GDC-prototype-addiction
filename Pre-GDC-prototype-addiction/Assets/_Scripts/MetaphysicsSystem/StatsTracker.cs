using System;
using System.Collections;
using System.Collections.Generic;
using MetaphysicsSystem;
using UnityEngine;

/// <summary>
/// record stats in game
/// </summary>
public class StatsTracker : MonoBehaviour
{
    public static Action<string, float> onBeforeValueChanged;
    // called when tracked value changes
    public static Action<string, float> onValueChanged;
    public static Action<string, float> onAfterValueChanged;


    private Dictionary<string, float> trackingStatsDict = new Dictionary<string, float>();

    private void OnEnable()
    {
        onValueChanged += ChangeTrackingStats;
    }

    private void OnDisable()
    {
        onValueChanged -= ChangeTrackingStats;
    }

    private void ChangeTrackingStats(string variableName, float value)
    {
         // check if the stat is being tracked, if not, starts tracking
         trackingStatsDict?.TryAdd(variableName, value);

         // change the stat value to a new one
         trackingStatsDict[variableName] = value;

         onAfterValueChanged?.Invoke(variableName, value);
    }
}
