using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class BridgeBoundary: MonoBehaviour
    {
        public void UpdateOffset(int step, float yOffset = 1.5f)
        {
            this.transform.localPosition = new Vector3(0, yOffset + step * 0.5f, step);
        }
    }
}