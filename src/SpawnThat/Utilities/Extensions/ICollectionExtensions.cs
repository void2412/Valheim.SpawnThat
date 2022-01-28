﻿using System.Collections.Generic;

namespace SpawnThat.Utilities.Extensions;

internal static class ICollectionExtensions
{
    public static void AddNullSafe<T>(this ICollection<T> collection, T newEntry)
        where T : class
    {
        if (collection is null)
        {
            return;
        }

        if (newEntry is null)
        {
            return;
        }

        if (newEntry is UnityEngine.Object unityObj &&
            !unityObj)
        {
            return;
        }

        collection.Add(newEntry);
    }
}
