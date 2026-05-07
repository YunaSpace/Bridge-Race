using System.Collections.Generic;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class BridgeHandler : MonoBehaviour
    {
        public List<Bridge> Bridges => bridges;

        [SerializeField] private List<Bridge> bridges = new();

        public void OnInitialize()
        {

        }

        public Bridge GetEmptyBridge()
        {
            var emptyBridges = new List<Bridge>();

            foreach (var bridge in bridges)
            {
                if (bridge.StartColor == ColorType.None)
                {
                    emptyBridges.Add(bridge);
                }
            }

            if (emptyBridges.Count == 0)
            {
                return null;
            }

            return emptyBridges.GetRandom();
        }

        public Bridge GetFewestColoredBridgeExcept(ColorType type)
        {
            var minAmount = int.MaxValue;
            Bridge minBridge = null;

            foreach (var bridge in bridges)
            {
                (ColorType Color, int Amount) minColor = bridge.GetFewestStairColor();

                if (minColor.Color == type)
                {
                    continue;
                }

                if (minColor.Amount < minAmount)
                {
                    minAmount = minColor.Amount;
                    minBridge = bridge;
                }
            }

            return minBridge;
        }
    
        public Bridge GetFullBridge()
        {
            var fullBridges = new List<Bridge>();

            foreach (var bridge in bridges)
            {
                if (bridge.IsFull && Random.Range(0, 5) > 1)
                {
                    fullBridges.Add(bridge);
                }
            }

            if (fullBridges.Count == 0)
            {
                return null;
            }

            return fullBridges.GetRandom();
        }
    }
}