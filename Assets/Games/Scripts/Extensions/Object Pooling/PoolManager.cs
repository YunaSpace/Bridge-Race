using System.Collections.Generic;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public static class PoolManager
    {
        private class Pool
        {
            public HashSet<PoolUnit> Actives => actives;
            public int Count => actives.Count + inactives.Count;
            public Transform Parent => parent;

            private Transform parent;
            private Queue<PoolUnit> inactives;
            private HashSet<PoolUnit> actives;
            private PoolUnit prefab;

            public Pool(PoolUnit prefab, int quantity, Transform parent)
            {
                this.parent = parent;
                this.prefab = prefab;
                inactives = new(quantity);
                actives = new();
            }

            public PoolUnit Spawn(Vector3 position, Quaternion rotation)
            {
                PoolUnit unit = null;

                if (inactives.Count > 0)
                {
                    if (Time.time >= inactives.Peek().TimeReady)
                    {
                        unit = inactives.Dequeue();
                    }
                }

                if (unit == null)
                {
                    unit = GameObject.Instantiate(prefab, parent);
                }

                actives.Add(unit);

                unit.transform.SetPositionAndRotation(position, rotation);
                unit.gameObject.SetActive(true);
                unit.OnSpawn();

                return unit;
            }

            public void Despawn(PoolUnit unit)
            {
                if (unit != null && unit.gameObject.activeSelf)
                {
                    unit.TimeReady = Time.time + unit.Cooldown;

                    unit.gameObject.SetActive(false);
                    unit.OnDespawn();
                    
                    inactives.Enqueue(unit);
                    actives.Remove(unit);
                }
            }

            public void Collect()
            {
                foreach (var unit in actives)
                {
                    if (unit != null)
                    {
                        unit.gameObject.SetActive(false);
                        unit.OnDespawn();
                        inactives.Enqueue(unit);
                    }
                }

                actives.Clear();
            }
        }

        private static Dictionary<int, Dictionary<PoolType, Pool>> localPools = new();
        private static Dictionary<PoolType, Pool> globalPools = new();

        public static void Preload(int ownerID, PoolUnit prefab, int quantity, Transform parent)
        {
            if (!localPools.ContainsKey(ownerID))
            {
                localPools[ownerID] = new Dictionary<PoolType, Pool>();
            }

            var characterPools = localPools[ownerID];
            if (!characterPools.ContainsKey(prefab.Type))
            {
                characterPools.Add(prefab.Type, new Pool(prefab, quantity, parent));
            }
        }

        public static T Spawn<T>(int ownerID, PoolType type, Vector3 position, Quaternion rotation) where T : PoolUnit
        {
            if (localPools.TryGetValue(ownerID, out var characterPools))
            {
                return characterPools[type].Spawn(position, rotation) as T;
            }
            Debug.LogError($"No local pool found for Owner {ownerID}");
            return null;
        }

        public static void Despawn(PoolUnit unit, int? ownerID = null)
        {
            if (unit == null) return;

            if (ownerID.HasValue && localPools.TryGetValue(ownerID.Value, out var characterPools))
            {
                characterPools[unit.Type].Despawn(unit);
            }
            else if (globalPools.TryGetValue(unit.Type, out var gPool))
            {
                gPool.Despawn(unit);
            }
        }

        public static void DespawnAll(int ownerID)
        {
            if (localPools.TryGetValue(ownerID, out var characterPools))
            {
                foreach (var pool in characterPools.Values)
                {
                    pool.Collect();
                }
            }
        }

        public static void Release(int ownerID)
        {
            if (localPools.ContainsKey(ownerID))
            {
                localPools.Remove(ownerID);
            }
        }
    }
}