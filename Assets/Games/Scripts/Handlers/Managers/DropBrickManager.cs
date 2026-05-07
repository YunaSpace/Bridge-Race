using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class DropBrickManager : MonoBehaviour
    {
        [SerializeField] private DropBrick brickPrefab;

        private int poolID;

        private void Awake()
        {
            poolID = gameObject.GetInstanceID();

            GlobalEvent.OnLevelCleared += OnLevelRestarted;
        }

        private void Start()
        {
            PoolManager.Preload(poolID, brickPrefab, 20, transform);
        }

        private void OnDestroy()
        {
            PoolManager.Release(poolID);

            GlobalEvent.OnLevelCleared -= OnLevelRestarted;
        }

        public void DropBrick(List<float3x2> dropTransforms)
        {
            for (int i = 0; i < dropTransforms.Count; i++)
            {
                var dropTransform = dropTransforms[i];

                var brick = PoolManager.Spawn<DropBrick>(poolID, PoolType.DropBrick, dropTransform.c0, Quaternion.Euler(dropTransform.c1));
                brick.OnInitialize();
                brick.Drop();
            }
        }

        public void CollectBrick(DropBrick brick)
        {
            PoolManager.Despawn(brick, poolID);
        }

        private void OnLevelRestarted()
        {
            PoolManager.DespawnAll(poolID);
        }
    }
}