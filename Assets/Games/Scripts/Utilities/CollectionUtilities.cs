using System.Collections.Generic;
using System;

namespace YunaSpace.BridgeRace
{
    public static class CollectionUtilities
    {
        public static T GetRandom<T>(this IList<T> list)
        {
            if (list.Count == 0) throw new IndexOutOfRangeException("List is empty!");
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
    }
}