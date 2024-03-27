using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Utils
{
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
    /// generate a number like x(1~9)000...
    /// </summary>
    public static int GenerateCleanNumber(List<int> maxNumberPlace)
    {
        int unitsPlace = Random.Range(1, 10);

        int numberPlace = maxNumberPlace[Random.Range(0, maxNumberPlace.Count)];

        int finalNumber = unitsPlace * numberPlace;

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