using System.Collections.Generic;
using System.Linq;
using _Scripts.ScratchCardGeneration.Utilities;
using UnityEngine;

namespace _Scripts.ScratchCardGeneration.PrizeGenerator
{
    public static class BasePrizeGenerator
    {
        // the possible prize when player wins
        private static Dictionary<int, float> _actualPrizeDistributions;

        private static System.Random random = new System.Random();

        /// <summary>
        /// generate the total prize of the current card
        /// </summary>
        public static float GeneratePrize(Dictionary<int, float> basePrizeDistributions, float costThreshold, float winningProbabilityOverThreshold)
        {
            _actualPrizeDistributions = Utils.DeepCopyDictionary(basePrizeDistributions);

            // if (GameManager.Instance.totalCostBeforeWinning >= costThreshold)
            // {
            //     int randIndex = Random.Range(1, basePrizeDistributions.Count);
            //     _actualPrizeDistributions = Utils.AdjustProbabilityRatio(basePrizeDistributions, basePrizeDistributions.Keys.ElementAt(randIndex), winningProbabilityOverThreshold);
            // }

            // return Utils.CalculateMultiProbability(_actualPrizeDistributions);

            // order the probability by ascending
            var sortedDistribution = _actualPrizeDistributions.OrderBy(pair => pair.Value);

            // float rand = Random.Range(0, 1f);

            double rand = random.NextDouble();

            Debug.Log($"rand: {rand}");
            float accumulatedProbability = 0;
            foreach (var d in sortedDistribution)
            {
                Debug.Log($"d value: {d.Value}");
                accumulatedProbability += d.Value;
                Debug.Log($"accumulatedProbability: {accumulatedProbability}");
                if (rand <= accumulatedProbability)
                {
                    Debug.Log("rand <= acc");
                    // return to the normal distribution after winning
                    // if (d.Key != 0)
                    // {
                    //     Debug.Log($"winning prize: {d.Key}");
                    //     _actualPrizeDistributions = Utils.DeepCopyDictionary(basePrizeDistributions);
                    //     GameManager.Instance.totalCostBeforeWinning = 0;
                    // }
                    return d.Key;
                }
            }
            return 0;
        }
    }
}
