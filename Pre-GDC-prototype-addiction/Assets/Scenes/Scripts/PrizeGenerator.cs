using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// TODO: add more kinds of probability model according to the rules of card
/// <summary>
/// calculate the prize of the following generated scratch card
/// </summary>
public class PrizeGenerator : MonoBehaviour
{
    [Serializable]
    public class PrizeDistribution
    {
        public float prize;
        public float probability;
    }

    public class CustomComparer : IComparer<PrizeDistribution>
    {
        public int Compare(PrizeDistribution x, PrizeDistribution y)
        {
            return x.probability.CompareTo(y.probability);
        }
    }

    [Header("Scratch Card Setting")]
    public GameObject scratchCardPrefab;
    public float price;
    public float responseRate;
    public List<PrizeDistribution> prizeDistributions;

    public float costThreshold;
    public float winningProbabilityAfterThreshold;

    // global among the same kind
    private float totalCostBeforeWinning = 0;
    private float currentWinningProbability;

    // [Header("Icon Prize Distribution")]

    void GenerateScratchCard()
    {

        AdjustWinningProbability();
        GenerateCurrentCardPrize();
    }

    /// <summary>
    /// adjust winning probability according to the previous spent
    /// </summary>
    void AdjustWinningProbability()
    {
        if (totalCostBeforeWinning > costThreshold)
        {
            currentWinningProbability = winningProbabilityAfterThreshold;
            totalCostBeforeWinning = 0;
        }
    }


    /// <summary>
    /// generate the total prize of the current card
    /// </summary>
    float GenerateCurrentCardPrize()
    {
        // win
        if (Random.Range(0, 1) <= currentWinningProbability)
        {
            // generate prize
            prizeDistributions.Sort(new CustomComparer());

            float rand = Random.value;
            foreach (var d in prizeDistributions)
            {
                if (rand <= d.probability)
                    return d.prize;
            }
        }
        return 0;
    }
}
