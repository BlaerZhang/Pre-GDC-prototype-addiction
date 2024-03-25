using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Utils
{

    public static void MultipleDistributedProbability()
    {

    }

    public static List<float> SplitNumbers(float totalNumber, int minParts, int maxParts, int minValue)
    {

        List<float> parts = new List<float>();
        int numberOfParts = Random.Range(minParts, maxParts + 1);
        float total = 0;

        for (int i = 0; i < numberOfParts - 1; i++)
        {
            float maxValue = totalNumber - total - (minValue * (numberOfParts - i - 1));
            float part = Random.Range(minValue, maxValue + 1);
            parts.Add(part);
            total += part;
        }

        // 将剩余的数字加入列表
        parts.Add(totalNumber - total);

        return parts;
    }
}
