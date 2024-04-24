using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Manager;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class FaceEventManager : SerializedMonoBehaviour
{
    // [HideInInspector] 
    public Dictionary<FaceEventType, int> faceEventDurationDict = new Dictionary<FaceEventType, int>()
    {
        { FaceEventType.Discount, 0 },
        { FaceEventType.Prize, 0 },
        { FaceEventType.WinningChance, 0 }
    };

    public static Action<FaceEventType> onFaceEventEnd;

    [HideInInspector] public ScratchCardTier discountTriggerTier;

    [HideInInspector] public ScratchCardTier prizeTriggerTier;

    [Serializable]
    public class tanhFormula
    {
        public float percentageOnTriggerTier = 50;
        public float deltaPercentage = 50;
        public float tanhSteepness = 0.5f;

        public List<Vector2> formulaTable = new List<Vector2>()
        {
            new Vector2(-5,0),
            new Vector2(-4,0),
            new Vector2(-3,0),
            new Vector2(-2,0),
            new Vector2(-1,0),
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(2,0),
            new Vector2(3,0),
            new Vector2(4,0),
            new Vector2(5,0),
        };
    }

    public tanhFormula discountFormula;

    void Start()
    {
        //Init
        // faceEventDurationDict.Add(FaceEventType.Discount, 0);
        // faceEventDurationDict.Add(FaceEventType.Prize, 0);
        // faceEventDurationDict.Add(FaceEventType.WinningChance, 0);
    }

    private void OnEnable()
    {
        FaceEventListener.onFaceEventTriggered += UpdateFaceEventInfo;
        onFaceEventEnd += ResetDiscountPrice;
        BuyCardManager.onChangeSubmissionStatus += DurationCountDown;
    }

    private void OnDisable()
    {
        FaceEventListener.onFaceEventTriggered -= UpdateFaceEventInfo;
        onFaceEventEnd += ResetDiscountPrice;
        BuyCardManager.onChangeSubmissionStatus -= DurationCountDown;
    }

    public void DurationCountDown()
    {
        foreach (var key in faceEventDurationDict.Keys.ToList())
        {
            faceEventDurationDict[key]--;
            if (faceEventDurationDict[key] < 0) faceEventDurationDict[key] = 0;

            //send end action if duration is 0
            if (faceEventDurationDict[key] == 0)
            {
                onFaceEventEnd?.Invoke(key);
            }
        }
    }

    public void ClearDuration()
    {
        foreach (var key in faceEventDurationDict.Keys.ToList())
        {
            faceEventDurationDict[key] = 0;
            onFaceEventEnd?.Invoke(key);
        }
    }

    public void UpdateFaceEventInfo(FaceEventType eventType, int eventDuration, ScratchCardTier triggerTier)
    {
        //update trigger tier
        switch (eventType)
        {
            case FaceEventType.Discount:
                discountTriggerTier = triggerTier;
                GameManager.Instance.lastPickPrice = CalculateDiscount(0, GameManager.Instance.lastPickOriginalPrice);
                break;
            case FaceEventType.Prize:
                prizeTriggerTier = triggerTier;
                break;
        }
        
        //update Duration
        faceEventDurationDict[eventType] = eventDuration + 1;
    }

    private void OnValidate()
    {
        for (int i = 0; i < discountFormula.formulaTable.Count; i++)
        {
            discountFormula.formulaTable[i] = new Vector2(discountFormula.formulaTable[i].x,
                discountFormula.deltaPercentage * (float)Math.Tanh(discountFormula.formulaTable[i].x * discountFormula.tanhSteepness) + discountFormula.percentageOnTriggerTier);
        }
    }

    public int CalculateDiscount(int tierDistance, int originalPrice)
    {
        float discountPrice = 1;
        int discountPriceRounded = 1;

        discountPrice = originalPrice
            * (discountFormula.deltaPercentage * (float)Math.Tanh(tierDistance * discountFormula.tanhSteepness) + discountFormula.percentageOnTriggerTier) / 100;

        int priceDigitsCount = Mathf.RoundToInt(discountPrice).ToString().Length;

        discountPriceRounded = Mathf.RoundToInt(discountPrice / MathF.Pow(10, priceDigitsCount - 2)) * (int)Mathf.Pow(10, priceDigitsCount - 2);
        
        //check if free
        discountPriceRounded = discountPriceRounded == 0 ? 1 : discountPriceRounded;
        
        return discountPriceRounded;
    }

    public void ResetDiscountPrice(FaceEventType faceEventType)
    {
        if (faceEventType != FaceEventType.Discount) return;
        GameManager.Instance.lastPickPrice = GameManager.Instance.lastPickOriginalPrice;
    }
}
