using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public static class VectorUtilities
    {
        public static Vector3 AddY(this Vector3 v, float y)
        {
            return new(v.x, v.y + y, v.z);
        }
        
        public static Vector3 SetY(this Vector3 v, float y)
        {
            return new(v.x, y, v.z);
        }
        
        public static Vector3 SetXZ(this Vector3 v, float x, float z)
        {
            return new(x, v.y, z);
        }
    }
}