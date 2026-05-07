using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class BridgeChecker : BridgeBoundary
    {
        [SerializeField] private ColorType colorType;

        protected Bridge bridge;

        public void Initialize(Bridge bridge, ColorType type)
        {
            this.bridge = bridge;
            this.colorType = type;
        }

        public void UpdateOffset(int step)
        {
            UpdateOffset(step, 0);
        }
    }
}