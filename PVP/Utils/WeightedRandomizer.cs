using System;
using System.Collections.Generic;

public static class WeightedRandomizer
{
    #region Class Methods

    public static WeightedRandomizer<T> From<T>(Dictionary<T, int> spawnRate)
    {
        return new WeightedRandomizer<T>(spawnRate);
    }

    #endregion Class Methods
}

public class WeightedRandomizer<T>
{
    #region Members

    private static Random _random = new Random();
    private Dictionary<T, int> _weights;

    #endregion Members

    #region Class Methods

    public WeightedRandomizer(Dictionary<T, int> weights)
    {
        _weights = weights;
    }

    public T TakeOne()
    {
        var sortedSpawnRate = Sort(_weights);
        int sum = 0;

        foreach (var spawn in _weights)
            sum += spawn.Value;

        int roll = _random.Next(0, sum);
        if (roll > sum)
            return default(T);

        T selected = sortedSpawnRate[sortedSpawnRate.Count - 1].Key;

        foreach (var spawn in sortedSpawnRate)
        {
            if (roll < spawn.Value)
            {
                selected = spawn.Key;
                break;
            }

            roll -= spawn.Value;
        }

        return selected;
    }

    private List<KeyValuePair<T, int>> Sort(Dictionary<T, int> weights)
    {
        var list = new List<KeyValuePair<T, int>>(weights);

        list.Sort
        (
            delegate (KeyValuePair<T, int> firstPair, KeyValuePair<T, int> nextPair)
            {
                return firstPair.Value.CompareTo(nextPair.Value);
            }
        );

        return list;
    }

    #endregion Class Methods
}