using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FaceEventManager : MonoBehaviour
{
    public Dictionary<FaceEventType, int> faceEventDurationDict = new Dictionary<FaceEventType, int>();

    public static Action<FaceEventType> onFaceEventEnd;

    void Start()
    {
        //Init
        faceEventDurationDict.Add(FaceEventType.Discount, 0);
        faceEventDurationDict.Add(FaceEventType.Prize, 0);
        faceEventDurationDict.Add(FaceEventType.WinningChance, 0);
    }
    
    public void DurationCountDown()
    {
        foreach (var key in faceEventDurationDict.Keys.ToList())
        {
            faceEventDurationDict[key]--;
            Mathf.Clamp(faceEventDurationDict[key], 0, Single.PositiveInfinity);

            //send end action if duration is 0
            if (faceEventDurationDict[key] == 0)
            {
                onFaceEventEnd?.Invoke(key);
            }
        }
    }
}
