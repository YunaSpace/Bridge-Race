using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class LevelBaker : MonoBehaviour
    {
        [SerializeField] private LevelConfigSO levelConfig;
        [SerializeField] private GoalPlatform goalPlatform;
        [SerializeField] private List<StageBuilder> stages = new();

        [SerializeField] private NavMeshSurface navMeshSurface;

        [Button("Bake")]
        public void Bake()
        {
            levelConfig.Stages.Clear();

            foreach (var stage in stages)
            {
                levelConfig.Stages.Add(stage.Bake());
            }

            levelConfig.GoalPosition = goalPlatform.transform.localPosition;

            navMeshSurface.BuildNavMesh();

            if (navMeshSurface.navMeshData != null)
            {
                string folderPath = "Assets/Games/Sources/Records/Nav Mesh Surfaces";

                string navMeshPath = $"{folderPath}/{levelConfig.name} Surface.asset";

                if (!AssetDatabase.Contains(navMeshSurface.navMeshData))
                {
                    AssetDatabase.CreateAsset(navMeshSurface.navMeshData, navMeshPath);
                }

                levelConfig.NavMeshData = navMeshSurface.navMeshData;
            }

            EditorUtility.SetDirty(levelConfig);
            AssetDatabase.SaveAssetIfDirty(levelConfig);
        }

        public void Apply(GoalPlatform goal, List<StageBuilder> stageBuilders)
        {
            stages.Clear();

            goalPlatform = goal;

            stages.AddRange(stageBuilders);
        }
    }
}