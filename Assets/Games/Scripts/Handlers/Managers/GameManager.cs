using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class GameManager : Singleton<GameManager>
    {
        public StageGroundManager GroundBrick;
        public FlyingBrickManager FlyingBrick;
        public StageBridgeManager StageBridge;
        public DropBrickManager DropBrick;
        public CharacterTeamManager CharacterTeam;
        public LevelBuilderManager LevelBuilder;
        public GameEnvironmentManager GameEnvironment;
    
        public ColorPaletteSO ColorPalette;
        public NameBadgeSO NameBadge;

        public Player Player;
        public CameraFocus CameraFocus;

        [SerializeField] private int startLevel;

        protected override void Awake()
        {
            base.Awake();

            Application.targetFrameRate = 120;

            OnInitialize();

            GlobalEvent.OnLevelFinished += OnFinish;
            GlobalEvent.OnLevelPaused += OnPause;
            GlobalEvent.OnLevelResumed += OnResume;
        }

        private void OnDestroy()
        {
            GlobalEvent.OnLevelFinished -= OnFinish;
            GlobalEvent.OnLevelPaused -= OnPause;
            GlobalEvent.OnLevelResumed -= OnResume;
        }

        private void Start()
        {
            Game.LevelBuilderManager.PlayLevel(startLevel);
            //GlobalEvent.OnLevelLoaded?.Invoke(startLevel);
            //GlobalEvent.OnLevelInitialized?.Invoke();
        }

        public void OnInitialize()
        {
            GlobalValue.IsLevelPaused = false;
        }

        public void OnPause()
        {
            GlobalValue.IsLevelPaused = true;
        }

        public void OnResume()
        {
            GlobalValue.IsLevelPaused = false;
        }

        public void OnFinish()
        {
            LevelBuilder.ShowWinner();

            Game.CharacterTeamManager.HideParticipant();

            View.CloseCanvas<CanvasGameplayView>();

            if (Game.CharacterTeamManager.IsPlayerVictory)
            { 
                View.OpenCanvas<CanvasVictoryView>();
            }
            else
            {
                View.OpenCanvas<CanvasLoseView>();
            }
        }
    }

    public static class Game
    {
        public static GameManager Manager => GameManager.Instance;
        public static FlyingBrickManager FlyingBrickManager => Manager.FlyingBrick;
        public static StageGroundManager StageGroundManager => Manager.GroundBrick;
        public static StageBridgeManager StageBridgeManager => Manager.StageBridge;
        public static DropBrickManager DropBrickManager => Manager.DropBrick;
        public static CharacterTeamManager CharacterTeamManager => Manager.CharacterTeam;
        public static LevelBuilderManager LevelBuilderManager => Manager.LevelBuilder;
        public static GameEnvironmentManager GameEnvironmentManager => Manager.GameEnvironment;

        public static ColorPaletteSO ColorPalette => Manager.ColorPalette;
        public static NameBadgeSO NameBadge => Manager.NameBadge;

        public static Player Player => Manager.Player;
        public static CameraFocus CameraFocus => Manager.CameraFocus;
    }
}