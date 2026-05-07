using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.Rendering;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class Bridge : MonoBehaviour
    {
        public ColorType StartColor => stairBricks[0].ColorType;
        public bool IsFull => stairBricks[^1].IsEnabled;
        public int NextStage { get => nextStage; set => nextStage = value; }

        [SerializeField] private StairBrick brickPrefab;
        [SerializeField] private Transform stairContainer;

        [SerializeField] private BridgeChecker checkerPrefab;
        [SerializeField] private Transform checkerContainer;
        
        [SerializeField] private BridgeBoundary bridgeBoundary;

        [SerializeField] private List<Transform> bridgeEdges = new();
        [SerializeField] private List<Transform> bridgePilars = new();
        [SerializeField] private Transform bridgeCollider;
        [SerializeField] private Transform bridgeEnd;

        [SerializeField] private Vector3 edgeOffset;
        [SerializeField] private Vector3 colliderOffset;
        [SerializeField] private Vector3 pilarOffset;
        [SerializeField] private int bridgeLength = 50;

        [SerializeField] private int nextStage;


        private const float baseScaleZ = 1f;

        private Dictionary<ColorType, BridgeChecker> bridgeCheckers = new();

        private List<StairBrick> stairBricks = new();

        private Dictionary<ColorType, int> colorCounts = new();

        private void OnValidate()
        {
            Build(bridgeLength);
        }

        private void Awake()
        {
            GlobalEvent.OnLevelInitialized += OnMatchInitialized;

            bridgeCollider.GetComponent<MeshRenderer>().enabled = false;
        }

        private void Start()
        {
            Build(bridgeLength);

            for (int i = 0; i < bridgeLength; i++)
            {
                StairBrick stair = Instantiate(brickPrefab, stairContainer);
                stair.transform.localPosition = new Vector3(0f, 0.75f + i * 0.5f, i);
                stair.UpdateStair(false);
                stair.SetBridge(this);

                stairBricks.Add(stair);
            }
        }

        private void OnDestroy()
        {
            GlobalEvent.OnLevelInitialized -= OnMatchInitialized;
        }

        public BridgeChecker GetBridgeChecker(ColorType colorType)
        {
            return bridgeCheckers[colorType];
        }

        public void SetBridgeLength(int length)
        {
            bridgeLength = length;

            Build(length);
        }

        public void RefeshBoundary(ColorType type, bool isPlayer)
        {
            if (StartColor != type)
            {
                if (bridgeCheckers.ContainsKey(type) == false)
                {
                    return;
                }

                bridgeCheckers[type].UpdateOffset(0);

                if (isPlayer)
                {
                    bridgeBoundary.UpdateOffset(0);
                }
            }
        }

        public void RefeshBoundary(StairBrick stair, ColorType type, bool isPlayer)
        {
            int index = stairBricks.IndexOf(stair) + 1;

            bridgeCheckers[type].UpdateOffset(index);

            if (isPlayer)
            {
                bridgeBoundary.UpdateOffset(index);
            }
        }

        public bool IsFullBridge(ColorType colorType)
        {
            return stairBricks[^1].gameObject.activeSelf && stairBricks[^1].ColorType == colorType;
        }

        public void LockForPlayer()
        {
            bridgeBoundary.gameObject.SetActive(false);
        }

        public (ColorType, int) GetFewestStairColor()
        {
            int minAmount = 0;
            ColorType minColor = ColorType.None;

            foreach (var pair in colorCounts)
            {
                if (pair.Value < minAmount)
                {
                    minAmount = pair.Value;
                    minColor = pair.Key;
                }
            }

            return (minColor, minAmount);
        }

        public void UpdateStairColorCount(StairBrick stair, ColorType oldColor, ColorType newColor)
        {
            if (colorCounts.ContainsKey(oldColor))
            {
                colorCounts[oldColor]--;
            }
            
            if (colorCounts.ContainsKey(newColor) == false)
            {
                colorCounts[newColor] = 0;
            }

            colorCounts[newColor]++;
        }

        public int GetStairAmount(ColorType type)
        {
            if (colorCounts.TryGetValue(type, out int amount) == false)
            {
                return 0;
            }

            return amount;
        }

        private void OnMatchInitialized()
        {
            CreateChecker();
        }

        private void Build(int length)
        {
            UpdateEdgeSize(bridgeEdges[0], length, new Vector3(-edgeOffset.x, edgeOffset.y, edgeOffset.z));
            UpdateEdgeSize(bridgeEdges[1], length, new Vector3(edgeOffset.x, edgeOffset.y, edgeOffset.z));

            UpdatePilarOffset(bridgePilars[0], length, new Vector3(-pilarOffset.x, pilarOffset.y, pilarOffset.z));
            UpdatePilarOffset(bridgePilars[1], length, new Vector3(pilarOffset.x, pilarOffset.y, pilarOffset.z));

            UpdateEdgeSize(bridgeCollider, length, new Vector3(colliderOffset.x, colliderOffset.y, colliderOffset.z));

            bridgeEnd.transform.localPosition = new Vector3(0f, 0.5f + length * 0.5f, length);
        }

        private void CreateChecker()
        {
            for (int i = 0; i < GlobalValue.MaxPlayerAmount; i++)
            {
                ColorType colortype = Game.CharacterTeamManager.TeamColors[i];

                BridgeChecker checker = Instantiate(checkerPrefab, checkerContainer);
                checker.UpdateOffset(0);
                checker.Initialize(this, colortype);

                bridgeCheckers.Add(colortype, checker);
            }
        }

        private void UpdateEdgeSize(Transform edge, int length, Vector3 offset)
        {
            float targetScaleZ = length * math.sqrt(1f + math.pow(0.5f, 2f));

            float scaleDifference = targetScaleZ - baseScaleZ;

            Vector3 newScale = edge.localScale;
            newScale.z = targetScaleZ;
            edge.localScale = newScale;

            edge.localPosition = edge.forward * (scaleDifference / 2f) + offset;
        }

        private void UpdatePilarOffset(Transform pilar, int length, Vector3 offset)
        {
            pilar.localPosition = new Vector3(0, length / 2f, length - 1) + offset;
        }
    }
}