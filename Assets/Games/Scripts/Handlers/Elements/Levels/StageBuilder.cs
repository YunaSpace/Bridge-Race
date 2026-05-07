using System.Collections.Generic;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class StageBuilder : MonoBehaviour
    {
        public GroundHandler GroundHandler => groundHandler;
        public BridgeHandler BridgeHandler => bridgeHandler;

        [SerializeField] private int stage;
        [SerializeField] private int bridgeLength;
        [SerializeField] private StagePlatform platform;
        [SerializeField] private GroundHandler groundHandler;
        [SerializeField] private List<Bridge> bridges = new();
        [SerializeField] private Transform locker;

        [SerializeField] private Bridge bridgePrefab;
        [SerializeField] private BridgeHandler bridgeHandler;

        public LevelStageData Bake()
        {
            var config = new LevelStageData();

            config.Stage = stage;
            config.BridgeLength = bridgeLength;
            config.StagePosition = transform.localPosition;
            config.LockerPosition = locker.localPosition;

            config.PlatformPoints = platform.PlatformPoints;
            config.PlatformEntrances = platform.PlatformEntrances;

            foreach (var bridge in bridges)
            {
                config.BridgeDatas.Add(new LevelBridgeData()
                {
                    Offset = bridge.transform.localPosition,
                    NextStage = bridge.NextStage
                });
            }

            return config;
        }

        public void Build(LevelStageData config)
        {
            stage = config.Stage;
            this.transform.localPosition = config.StagePosition;
            locker.localPosition = config.LockerPosition;

            bridgeLength = config.BridgeLength;

            platform.BuildPlatform(config.PlatformPoints, config.PlatformEntrances);

            for (int i = 0; i < config.BridgeDatas.Count; i++)
            {
                var bridge = Instantiate(bridgePrefab, bridgeHandler.transform);
                bridge.transform.localPosition = config.BridgeDatas[i].Offset;
                bridge.NextStage = config.BridgeDatas[i].NextStage;
                bridge.SetBridgeLength(config.BridgeLength);

                bridges.Add(bridge);
                bridgeHandler.Bridges.Add(bridge);
            }

            bridgeHandler.OnInitialize();
        }
    }
}