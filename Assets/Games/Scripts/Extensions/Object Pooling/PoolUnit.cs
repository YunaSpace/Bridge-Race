using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public abstract class PoolUnit : MonoBehaviour
    {
        public Transform Transform => transform;

        public abstract float Cooldown { get; }
        public float TimeReady { get; set; }

        public PoolType Type;

        public virtual void OnSpawn()
        {

        }

        public virtual void OnDespawn()
        {

        }
    }
}