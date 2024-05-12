using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.ScratchCardGeneration.Utilities
{
    public static class Utils
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int count = list.Count;
            while (count > 1)
            {
                count--;
                global::System.Random random = new global::System.Random();
                int randomIndex = random.Next(count + 1);
                (list[randomIndex], list[count]) = (list[count], list[randomIndex]);
            }
        }

        public static T GetRandomElementFromList<T>(List<T> list)
        {
            int randIndex = Random.Range(0, list.Count);
            return list[randIndex];
        }

        public static T CalculateMultiProbability<T>(Dictionary<T, float> probabilityDict)
        {
            var sortedDistribution = probabilityDict.OrderBy(pair => pair.Value);

            float rand = Random.value;
            float accumulatedProbability = 0;
            foreach (var d in sortedDistribution)
            {
                accumulatedProbability += d.Value;
                if (rand <= accumulatedProbability)
                {
                    // Debug.Log($"accumulatedProbability: {accumulatedProbability}");

                    return d.Key;
                }
            }

            return default;
        }

        public static Vector2Int SelectRandomGridFromMatrix(int matrixRow, int matrixColumn)
        {
            return new Vector2Int(Random.Range(0, matrixRow), Random.Range(0, matrixColumn));
        }

        /// <summary>
        /// transform a list into variable matrix
        /// </summary>
        /// <param name="list"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static VariableMatrix<T> ListToVariableMatrix<T>(List<T> list, int row, int column)
        {
            VariableMatrix<T> matrix = new VariableMatrix<T>(row, column);
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    int currentIndex = i * column + j;
                    matrix.SetElement(i, j, list[currentIndex]);
                }
            }

            return matrix;
        }

        /// <summary>
        /// get a random number from a range, but exclude certain numbers
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="exclusions"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static int GetRandomWithExclusions(int min, int max, HashSet<int> exclusions)
        {
            List<int> availableNumbers = new List<int>();
            for (int i = min; i < max; i++)
            {
                if (!exclusions.Contains(i))
                {
                    availableNumbers.Add(i);
                }
            }

            if (availableNumbers.Count == 0)
            {
                throw new InvalidOperationException("No numbers available for random selection.");
            }

            int randomIndex = Random.Range(0, availableNumbers.Count);
            return availableNumbers[randomIndex];
        }

        public static List<T> DeepCopyList<T>(List<T> original)
        {
            List<T> copy = new ();
            foreach (var element in original)
            {
                copy.Add(element);
            }
            return copy;
        }

        public static Dictionary<TKey, TValue> DeepCopyDictionary<TKey, TValue>(Dictionary<TKey, TValue> original)
        {
            Dictionary<TKey, TValue> copy = new Dictionary<TKey, TValue>();
            foreach (KeyValuePair<TKey, TValue> entry in original)
            {
                copy.Add(entry.Key, entry.Value);
            }
            return copy;
        }

        /// <summary>
        /// dynamically adjust each prize's winning probability proportionally
        /// </summary>
        /// <param name="probabilityDic"></param>
        /// <param name="adjustingName"></param>
        /// <param name="newProbability"></param>
        public static Dictionary<int, float> AdjustProbabilityRatio(Dictionary<int, float> originalProbabilityDic, int adjustingName, float newProbability)
        {
            if (!originalProbabilityDic.Keys.Contains(adjustingName) || newProbability > 1) return null;

            Dictionary<int, float> copy = new Dictionary<int, float>();

            float oldRestProbability = 1 - originalProbabilityDic[adjustingName];
            float newRestProbability = 1 - newProbability;
            float adjustRatio = newRestProbability / oldRestProbability;

            foreach (var kv in originalProbabilityDic)
            {
                if (kv.Key.Equals(adjustingName))
                {
                    copy.Add(adjustingName, newProbability);
                    continue;
                }

                copy.Add(kv.Key, kv.Value * adjustRatio);
            }
            return copy;
        }

        /// <summary>
        /// how many times of 10 this number has
        /// </summary>
        public static int GetNumberPlace(float number)
        {
            int numberPlace = 0;

            int currentNumberPlace = (int)number;

            while (currentNumberPlace / 10 >= 1)
            {
                numberPlace++;
                currentNumberPlace /= 10;
            }

            return numberPlace;
        }

        /// <summary>
        /// generate a number like x(1~9)000...
        /// </summary>
        public static int GenerateCleanNumber(int minNumberPlace, int maxNumberPlace)
        {
            int unitsPlace = Random.Range(1, 10);

            int randNumberPlace = Random.Range(minNumberPlace, maxNumberPlace + 1);

            int finalNumber = unitsPlace * (int)Mathf.Pow(10, randNumberPlace);

            return finalNumber;
        }

        public static List<float> SplitNumbers(float totalNumber, int minParts, int maxParts, int minValue)
        {
            if (minValue * minParts > totalNumber)
            {
                // 如果最小分割数量乘以最小值超过总数，则减少分割数量
                minParts = Mathf.Max(1, Mathf.FloorToInt(totalNumber / minValue));
            }

            maxParts = Mathf.Max(minParts, Mathf.Min(maxParts, Mathf.FloorToInt(totalNumber / minValue)));
            List<float> valueParts = new List<float>();
            int numberOfParts = Random.Range(minParts, maxParts + 1);

            float remainingNumber = totalNumber;

            for (int i = 0; i < numberOfParts - 1; i++)
            {
                // 计算这部分可能的最大值
                float maxForThisPart = remainingNumber - minValue * (numberOfParts - i - 1);
                // 确保这部分的最大值不小于最小值
                maxForThisPart = Mathf.Max(minValue, maxForThisPart);

                // 生成一个随机值
                float part = Random.Range(1, Mathf.FloorToInt(maxForThisPart / minValue) + 1) * minValue;
                valueParts.Add(part);

                remainingNumber -= part;
            }

            // 添加剩余的数值
            if (remainingNumber > 0) valueParts.Add(remainingNumber);

            return valueParts;
        }
    }
}
