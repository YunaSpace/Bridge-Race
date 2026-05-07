using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace YunaSpace.BridgeRace
{
    [Serializable]
    public class LevelBridgeData
    {
        public Vector3 Offset;
        public int NextStage;
    }

    [Serializable]
    public class LevelStageData
    {
        public int Stage;
        public int BridgeLength;

        public Vector3 StagePosition;
        public Vector3 LockerPosition;

        public List<Vector3> PlatformPoints = new();
        public List<float> PlatformEntrances = new();
        public List<LevelBridgeData> BridgeDatas = new();
    }


    [CreateAssetMenu(fileName = "Level Config SO", menuName = "Bridge Race/Level Config SO")]
    public class LevelConfigSO : ScriptableObject
    {
        public int Level;
        public List<LevelStageData> Stages = new();
        
        public Vector3 GoalPosition;
        public NavMeshData NavMeshData;
    }
}