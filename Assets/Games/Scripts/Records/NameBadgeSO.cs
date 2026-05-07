using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    [CreateAssetMenu(fileName = "Name Badge SO", menuName = "Bridge Race/Name Badge SO")]
    public class NameBadgeSO : ScriptableObject
    {
        public List<string> Names = new();

        public string[] GetRandomNames()
        {
            return Names.OrderBy(_ => Random.value).Take(GlobalValue.MaxPlayerAmount - 1).ToArray();
        }
    }
}
