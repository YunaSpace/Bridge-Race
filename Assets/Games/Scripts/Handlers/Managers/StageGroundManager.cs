using System.Collections.Generic;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class StageGroundManager : MonoBehaviour
    {
        public List<GroundHandler> GroundHandlers
        {
            get => groundHandlers;
            set => groundHandlers = value;
        }

        [SerializeField] private List<GroundHandler> groundHandlers = new();

        private ColorPaletteSO colorPalette => GameManager.Instance.ColorPalette;

        private void Awake()
        {
            colorPalette.InitializeMaterial();

            GlobalEvent.OnLevelInitialized += OnMatchInitialized;
        }
        
        private void OnDestroy()
        {
            GlobalEvent.OnLevelInitialized -= OnMatchInitialized;
        }


        public GroundBrick FindNearestBrick(Vector3 worldPosition, ColorType targetColor, int stage = 0)
        {
            return groundHandlers[stage].FindNearestBrick(worldPosition, targetColor);
        }

        public void CollectBrick(GroundBrick brick, int stage = 0)
        {
            groundHandlers[stage].CollectBrick(brick);
        }

        public void ShowAllBrickOfColor(ColorType colorType, int stage)
        {
            groundHandlers[stage].ShowAllBrickOfColor(colorType);
        }

        private void OnMatchInitialized()
        {
            CreatePlayground();
        }

        private void CreatePlayground()
        {
            for (int i = 0; i < groundHandlers.Count; i++)
            {
                GroundHandler groundHandler = groundHandlers[i];

                groundHandler.CreateGroundBrick(i == 0);
            }
        }
    }
}