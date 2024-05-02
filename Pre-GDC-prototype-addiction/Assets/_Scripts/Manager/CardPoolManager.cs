using System;
using System.Collections;
using System.Collections.Generic;
using Interaction;
using Manager;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardPoolManager : SerializedMonoBehaviour
{
    [DictionaryDrawerSettings(KeyLabel = "Card Prefab", ValueLabel = "Weight in Pool")]
    public Dictionary<SelectableScratchCard, int> cardPrefabsDict;
    
    private Dictionary<SelectableScratchCard, int> initialPoolWeightDict;

    [Serializable] public class CardsStatsPerTier
    {
        public ScratchCardTier tier;
        public bool poolRefreshLock = false;
        public List<SelectableScratchCard> currentCards;
    }

    // [Serializable] public class EventTriggerWeightPerFaceType
    // {
    //     public FaceType faceType;
    //     public int noEventWeight;
    //     public int discountWeight;
    //     public int event2Weight;
    //     public int event3Weight;
    // }

    [DictionaryDrawerSettings(KeyLabel = "Face Type", ValueLabel = "Face Event Probability")]
    public Dictionary<FaceType, Dictionary<FaceEventType, float>> eventTriggerWeightPerFaceTypeDict;
    
    [HideInInspector] public List<CardsStatsPerTier> cardStatsList;
    
    [ContextMenu(nameof(CreateData))] private void CreateData()
    {
        if (eventTriggerWeightPerFaceTypeDict.Count > 0) return;
        eventTriggerWeightPerFaceTypeDict = new Dictionary<FaceType, Dictionary<FaceEventType, float>>()
        {
            {
                FaceType.Original, new Dictionary<FaceEventType, float>()
                {
                    { FaceEventType.NoEvent, 0.75f },
                    { FaceEventType.Discount, 0.08f },
                    { FaceEventType.WinningChance, 0.08f },
                    { FaceEventType.Prize, 0.09f },
                }
            },

            {
                FaceType.B, new Dictionary<FaceEventType, float>()
                {
                    { FaceEventType.NoEvent, 0.9f },
                    { FaceEventType.Discount, 0.1f },
                    { FaceEventType.WinningChance, 0 },
                    { FaceEventType.Prize, 0 },
                }
            },

            {
                FaceType.C, new Dictionary<FaceEventType, float>()
                {
                    { FaceEventType.NoEvent, 0.8f },
                    { FaceEventType.Discount, 0.04f },
                    { FaceEventType.WinningChance, 0.08f },
                    { FaceEventType.Prize, 0.08f },
                }
            },

            {
                FaceType.D, new Dictionary<FaceEventType, float>()
                {
                    { FaceEventType.NoEvent, 0.7f },
                    { FaceEventType.Discount, 0.1f },
                    { FaceEventType.WinningChance, 0.1f },
                    { FaceEventType.Prize, 0.1f },
                }
            },

            {
                FaceType.E, new Dictionary<FaceEventType, float>()
                {
                    { FaceEventType.NoEvent, 0.6f },
                    { FaceEventType.Discount, 0 },
                    { FaceEventType.WinningChance, 0.2f },
                    { FaceEventType.Prize, 0.2f },
                }
            },
        };
    }
    
    private void OnEnable()
    {
        BuyCardManager.onChangeSubmissionStatus += ResetRefreshLock;
    }

    private void OnDisable()
    {
        BuyCardManager.onChangeSubmissionStatus -= ResetRefreshLock;
    }

    private void Start()
    {
        initialPoolWeightDict = cardPrefabsDict;
        
        //set up cardStatsList
        for (int tier = 0; tier < System.Enum.GetValues(typeof(ScratchCardTier)).Length; tier++)
        {
            CardsStatsPerTier cardStats = new CardsStatsPerTier();
            cardStats.tier = (ScratchCardTier)tier;
            cardStatsList.Add(cardStats);
        }
    }

    public void ResetWeightToInitial()
    {
        cardPrefabsDict = initialPoolWeightDict;
    }

    public void ResetRefreshLock()
    {
        cardStatsList[(int)GameManager.Instance.lastPickTier].poolRefreshLock = false;
    }

    public List<SelectableScratchCard> CreateCardPool(ScratchCardTier tier)
    {
        List<SelectableScratchCard> cardsToBuy = new List<SelectableScratchCard>();
        
        if (cardStatsList[(int)tier].poolRefreshLock)
        {
            //return previous pool
            cardsToBuy = cardStatsList[(int)tier].currentCards;
        }
        else
        {
            //set up spawn pool to draw from
            List<SelectableScratchCard> spawnPool = new List<SelectableScratchCard>();
            foreach (var cardPrefabKeyValuePair in cardPrefabsDict)
            {
                for (int i = 0; i < cardPrefabKeyValuePair.Value; i++)
                {
                    spawnPool.Add(cardPrefabKeyValuePair.Key);
                }
            }
            
            //draw 5
            for (int i = 0; i < 5; i++)
            {
                cardsToBuy.Add(spawnPool[Random.Range(0, spawnPool.Count)]);
            }
            
            //lock refresh
            cardStatsList[(int)tier].poolRefreshLock = true;
            
            //update current cards
            cardStatsList[(int)tier].currentCards = cardsToBuy;
        }
        
        return cardsToBuy;
    }
}
