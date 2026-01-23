using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public static class ListExtensions
{
    private static readonly Random _random = new Random();

    public static T GetRandom<T>(this IList<T> list)
    {
        if (list == null || list.Count == 0)
            throw new InvalidOperationException("Cannot get random element from a null or empty list.");

        return list[_random.Next(list.Count)];
    }
}
