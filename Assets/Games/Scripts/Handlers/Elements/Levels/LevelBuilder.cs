using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class LevelBuilder : MonoBehaviour
    {
        public GoalPlatform GoalPlatform => goalPlatform;
        public int MaxStage => maxStage;

        public List<GroundHandler> GroundHandlers => groundHandlers;
        public List<BridgeHandler> BridgeHandlers => bridgeHandlers;

        [SerializeField] private LevelConfigSO levelConfig;

        [SerializeField] private NavMeshSurface navMeshSurface;

        [SerializeField] private StageBuilder stagePrefab;
        [SerializeField] private GoalPlatform goalPrefab;

        [SerializeField] private LevelBaker levelBaker;

        private List<GroundHandler> groundHandlers = new();
        private List<BridgeHandler> bridgeHandlers = new();

        private GoalPlatform goalPlatform;
        private int maxStage;

        public void BuildLevel(LevelConfigSO record)
        {
            levelConfig = record;

            Build();
        }

        [Button("Build")]
        public void Build()
        {
            Clear();

            groundHandlers.Clear();
            bridgeHandlers.Clear();

            List<StageBuilder> stageBuilders = new();

            maxStage = 0;

            foreach (var stageConfig in levelConfig.Stages)
            {
                var stage = Instantiate(stagePrefab, transform);
                stage.Build(stageConfig);

                groundHandlers.Add(stage.GroundHandler);
                bridgeHandlers.Add(stage.BridgeHandler);

                maxStage++;

                stageBuilders.Add(stage);
            }

            goalPlatform = Instantiate(goalPrefab, transform);
            goalPlatform.transform.localPosition = levelConfig.GoalPosition;
            goalPlatform.GeneratePlatform();

            navMeshSurface.RemoveData();
            navMeshSurface.navMeshData = null;
            navMeshSurface.navMeshData = levelConfig.NavMeshData;
            navMeshSurface.AddData();

            if (levelBaker != null)
            {
                levelBaker.Apply(goalPlatform, stageBuilders);
            }
        }

        [Button("Clear")]
        public void Clear()
        {
            this.transform.DestroyAllChildrenImmediately();
        }
    }
}