using UnityEngine;
using UnityEngine.UI;
using YunaSpace.BridgeRace;

namespace YunaSpace.BridgeRace
{
    public class CanvasLoseView : ViewCanvas<CanvasLoseView>
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        private void Awake()
        {
            restartButton.onClick.AddListener(OnRestartLevelClicked);
            menuButton.onClick.AddListener(OnMenuButtonClicked);

            GlobalEvent.OnLevelMasked += OnLevelMasked;
        }

        private void OnDestroy()
        {
            restartButton.onClick.RemoveAllListeners();
            menuButton.onClick.RemoveAllListeners();

            GlobalEvent.OnLevelMasked -= OnLevelMasked;
        }

        private void OnRestartLevelClicked()
        {
            Close();

            View.OpenCanvas<CanvasMaskView>();
        }

        private void OnMenuButtonClicked()
        {
            Close();

            View.OpenCanvas<CanvasMaskView>();
        }

        private void OnLevelMasked()
        {
            Game.LevelBuilderManager.RestartLevel();

            View.OpenCanvas<CanvasMenuView>();
        }
    }
}