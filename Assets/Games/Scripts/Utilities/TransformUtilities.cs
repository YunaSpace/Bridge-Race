using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public static class TransformUtilities
    {
        public static void DestroyAllChildren(this Transform gameObject)
        {
            foreach (Transform child in gameObject.transform)
            {
                Object.Destroy(child.gameObject);
            }
        }

        public static void DestroyAllChildrenImmediately(this Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(parent.GetChild(i).gameObject);
            }
        }

    }
}