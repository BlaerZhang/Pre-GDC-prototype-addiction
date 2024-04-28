using System.Collections.Generic;
using MetaphysicsSystem;
using UnityEngine;

public class RequirementComparer : MonoBehaviour
{
    public List<ScriptableMetaphysics> metaphysicsList;

    private void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        StatsTracker.onAfterValueChanged += CheckIfTriggerResponseEvent;
    }

    private void OnDisable()
    {
        StatsTracker.onAfterValueChanged -= CheckIfTriggerResponseEvent;
    }

    private void Init()
    {
        foreach (var metaPhysics in metaphysicsList)
        {
            metaPhysics.ResetData();
        }
    }

    /// <summary>
    /// check if the metaphysics triggered after relevant value changes
    /// </summary>
    private void CheckIfTriggerResponseEvent(string changedVariableName, float variableValue)
    {
        // print("CheckIfTriggerResponseEvent");

        if (metaphysicsList == null)
        {
            Debug.LogError("Metaphysics List is null!");
            return;
        }

        foreach (var metaPhysics in metaphysicsList)
        {
            int matchedNum = 0;
            // if contains the variable that changed, means possibly the requirement would be met
            if (metaPhysics.metaphysicsRequirement.ContainsKey(changedVariableName))
            {
                // check if the variable change whether it could trigger the metaphysics effect
                foreach (var requirement in metaPhysics.metaphysicsRequirement)
                {
                    // change the match status, if this is the one that changes
                    if (requirement.Key.Equals(changedVariableName))
                    {
                        requirement.Value.isMatched = CompareRequirements(variableValue, requirement.Value.value, requirement.Value.comparator);
                    }

                    // if the requirement is matched, add the number of requirements matched
                    if (requirement.Value.isMatched)
                    {
                        matchedNum++;
                    }
                }

                if (matchedNum == metaPhysics.metaphysicsRequirement.Count)
                {
                    // trigger response event
                    if (metaPhysics.isRepeatable || !metaPhysics.hasTriggered)
                    {
                        metaPhysics.response.Raise();
                        metaPhysics.hasTriggered = true;
                    }
                }
            }
        }
    }

    /// <summary>
    ///  compare two floats
    /// </summary>
    /// <param name="currentVariable"></param>
    /// <param name="comparator"></param>
    /// <param name="requiredValue"></param>
    /// <returns>return true if meets the requirement</returns>
    private bool CompareRequirements(float currentVariable, float requiredValue, Comparator comparator)
    {
        switch (comparator)
        {
            case Comparator.Equal:
                if (Mathf.Approximately(requiredValue, currentVariable)) return true;
                break;
            case Comparator.Large:
                if (!Mathf.Approximately(requiredValue, currentVariable) & currentVariable > requiredValue) return true;
                break;
            case Comparator.Smaller:
                if (!Mathf.Approximately(requiredValue, currentVariable) & currentVariable < requiredValue) return true;
                break;
            case Comparator.LargeEqual:
                if (Mathf.Approximately(requiredValue, currentVariable) || currentVariable > requiredValue) return true;
                break;
            case Comparator.SmallerEqual:
                if (Mathf.Approximately(requiredValue, currentVariable) || currentVariable < requiredValue) return true;
                break;
        }

        return false;
    }
}
