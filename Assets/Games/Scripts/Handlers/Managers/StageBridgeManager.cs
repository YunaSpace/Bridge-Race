using System.Collections.Generic;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class StageBridgeManager : MonoBehaviour
    {
        public List<BridgeHandler> BridgeHandlers
        {
            get => bridgeHandlers;
            set => bridgeHandlers = value;
        }

        [SerializeField] private List<BridgeHandler> bridgeHandlers = new();

        public Bridge GetEmptyBridge(int stage = 0)
        {
            return bridgeHandlers[stage].GetEmptyBridge();
        }

        public Bridge GetFewestColoredBridgeExcept(ColorType type, int stage = 0)
        {
            return bridgeHandlers[stage].GetFewestColoredBridgeExcept(type);
        }

        public Bridge GetRandomFullBridge(int stage = 0)
        {
            return bridgeHandlers[stage].GetFullBridge();
        }
    }
}