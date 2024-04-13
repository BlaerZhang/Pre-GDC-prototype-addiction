using System;
using System.Collections;
using System.Collections.Generic;
using Interaction;
using Manager;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;
using Interaction;

public class CardPoolManager : SerializedMonoBehaviour
{
    public Dictionary<Draggable, int> cardPrefabsDict;
    private Dictionary<Draggable, int> initialPoolWeightDict;

    [Serializable] public class CardsStatsPerTier
    {
        public ScratchCardTier tier;
        public bool poolRefreshLock = false;
        public List<Draggable> currentCards;
    }

    [HideInInspector] public List<CardsStatsPerTier> cardStatsList;

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

    public List<Draggable> CreateCardPool(ScratchCardTier tier)
    {
        List<Draggable> cardsToBuy = new List<Draggable>();
        
        if (cardStatsList[(int)tier].poolRefreshLock)
        {
            //return previous pool
            cardsToBuy = cardStatsList[(int)tier].currentCards;
        }
        else
        {
            //set up spawn pool to draw from
            List<Draggable> spawnPool = new List<Draggable>();
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
