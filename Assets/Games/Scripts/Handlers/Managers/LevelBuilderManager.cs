using System.Collections.Generic;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class LevelBuilderManager : MonoBehaviour
    {
        public GoalPlatform GoalPlatform => goalPlatform;
        public int MaxStage => maxStage;
        public int CurrentLevel => currentLevel;

        [SerializeField] private List<LevelConfigSO> levelRecords = new();
        [SerializeField] private LevelBuilder levelBuilder;

        [SerializeField] private int currentLevel;
        
        private GoalPlatform goalPlatform;
        private int maxStage;

        private void Awake()
        {
            GlobalEvent.OnLevelLoaded += LoadLevel;
        }
        
        private void OnDestroy()
        {
            GlobalEvent.OnLevelLoaded -= LoadLevel;
        }

        public void LoadLevel(int level)
        {
            currentLevel = level;

            var levelRecord = levelRecords[level - 1];

            levelBuilder.BuildLevel(levelRecord);

            maxStage = levelBuilder.MaxStage;

            goalPlatform = levelBuilder.GoalPlatform;
            goalPlatform.OnInitialize();

            Game.StageGroundManager.GroundHandlers = levelBuilder.GroundHandlers;
            Game.StageBridgeManager.BridgeHandlers = levelBuilder.BridgeHandlers;
        }

        public void ShowWinner()
        {
            var winners = Game.CharacterTeamManager.GetWinner();

            goalPlatform.ShowWinner(winners);
        }

        [Button]
        public void RestartLevel()
        {
            PlayLevel(currentLevel);
        }

        [Button]
        public void NextLevel()
        {
            PlayLevel(currentLevel + 1);
        }
        
        
        public void PlayLevel(int level)
        {
            GlobalValue.IsLevelStarted = false;

            GlobalEvent.OnLevelCleared?.Invoke();

            GlobalEvent.OnLevelLoaded?.Invoke(level);
            GlobalEvent.OnLevelInitialized?.Invoke();

            Game.CharacterTeamManager.ShowParticipant();

            GlobalEvent.OnLevelResumed?.Invoke();
        }
    }
}