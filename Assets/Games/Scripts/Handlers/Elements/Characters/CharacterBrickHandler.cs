using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class CharacterBrickHandler : MonoBehaviour
    {
        public Character Manager;

        [SerializeField] private ColorPaletteSO colorPalette;

        [SerializeField] private PoolUnit brickPrefab;
        [SerializeField] private Transform stackContainer;
        [SerializeField] private Transform stackTop;
        [SerializeField] private Transform brickContainer;

        [SerializeField] private new Collider collider;
        
        private int poolID;
        private List<PoolUnit> activeBricks = new();

        private List<float3x2> brickTransforms = new();

        private void Start()
        {
            poolID = gameObject.GetInstanceID();

            PoolManager.Preload(poolID, brickPrefab, 15, stackContainer);
        }

        private void OnDestroy()
        {
            PoolManager.Release(poolID);
        }

        private void OnTriggerEnter(Collider other)
        {
            bool isValidToCollectBrick = Manager.BrickCount < GlobalValue.MaxBrickCarried && Manager.IsStumbling == false;

            if (isValidToCollectBrick)
            {
                if (other.CompareTag("GroundBrick"))
                {
                    var brick = other.GetComponent<GroundBrick>();

                    OnGroundBrickCollect(brick);
                }
                else if (other.CompareTag("DropBrick"))
                {
                    var brick = other.GetComponentInParent<DropBrick>();

                    OnDropBrickCollect(brick);
                }
            }
        }

        public void OnInitialize()
        {
            for (int i = 0; i < activeBricks.Count; i++)
            {
                var brick = activeBricks[i];

                PoolManager.Despawn(brick, poolID);
            }

            activeBricks.Clear();
        }

        public void EnableCollider(bool enabled)
        {
            collider.enabled = enabled;
        }

        public void ShowBrick(bool toShow)
        {
            var y = GlobalValue.BrickStackSpace * Manager.BrickCount;

            stackTop.localPosition = new(0, y, 0);

            if (toShow)
            {
                var brick = PoolManager.Spawn<PoolUnit>(poolID, PoolType.CarryingBrick, Vector3.zero, Quaternion.identity) as CarryingBrick;
                brick.Transform.SetLocalPositionAndRotation(new(0, y - GlobalValue.BrickStackSpace, 0), Quaternion.identity);
                brick.SetColor(Manager.ColorType);
                brick.ShowAnimation();

                activeBricks.Add(brick);
            }
            else
            {
                if (activeBricks.Count > 0)
                {
                    var lastBrick = activeBricks[^1];

                    activeBricks.Remove(lastBrick);
                    PoolManager.Despawn(lastBrick, poolID);
                }
            }
        }

        public List<float3x2> GetDroppedBrickTransform(bool isPlayer)
        {
            brickTransforms.Clear();

            if (activeBricks.Count < 1)
            {
                return brickTransforms;
            }

            var shiftHeight = 0f;

            for (int i = 0; i < activeBricks.Count; i++)
            {
                var brick = activeBricks[i];

                if (i == 0)
                {
                    shiftHeight = brickContainer.position.y - brick.transform.position.y;
                }

                brickTransforms.Add(new(brick.transform.position.AddY(shiftHeight), brick.transform.rotation.eulerAngles));
            }

            activeBricks.Clear();

            PoolManager.DespawnAll(poolID);

            stackTop.localPosition = Vector3.zero;

            return brickTransforms;
        }
        
        private void OnGroundBrickCollect(GroundBrick brick)
        {
            if (brick.ColorType == Manager.ColorType)
            {
                if (Manager.IsBuilding)
                {
                    Manager.SetIsBuilding(true);
                }

                Game.StageGroundManager.CollectBrick(brick, Manager.CurrentStage);
                Game.FlyingBrickManager.ShowFlying(brick.ColorType, brick.transform.position, stackTop, () => Manager.AddBrick(true));
            }

            Manager.OnBrickCountChanged();
        }

        private void OnDropBrickCollect(DropBrick brick)
        {
            if (brick != null)
            {
                if (Manager.IsBuilding)
                {
                    Manager.SetIsBuilding(true);
                }

                Game.DropBrickManager.CollectBrick(brick);

                Game.FlyingBrickManager.ShowFlying(Manager.ColorType, brick.transform.position, stackTop, () => Manager.AddBrick(true));

                Manager.OnBrickCountChanged();
            }
        }
    }
}