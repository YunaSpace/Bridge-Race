using UnityEngine;
using UnityEngine.UI;

namespace YunaSpace.BridgeRace
{
    public class CanvasSettingsView : ViewCanvas<CanvasSettingsView>
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button homeButton;

        private void Awake()
        {
            resumeButton.onClick.AddListener(OnResumeButtonClicked);
            restartButton.onClick.AddListener(OnRestartButtonClicked);
            homeButton.onClick.AddListener(OnHomeButtonClicked);

            GlobalEvent.OnLevelMasked += OnLevelMasked;
        }

        private void OnDestroy()
        {
            resumeButton.onClick.RemoveAllListeners();
            restartButton.onClick.RemoveAllListeners();
            homeButton.onClick.RemoveAllListeners();

            GlobalEvent.OnLevelMasked -= OnLevelMasked;
        }

        private void OnResumeButtonClicked()
        {
            Close();

            View.OpenCanvas<CanvasCountdownView>();
        }

        private void OnRestartButtonClicked()
        {
            View.CloseAllCanvas();

            View.OpenCanvas<CanvasMaskView>();
        }

        private void OnHomeButtonClicked()
        {
            View.CloseAllCanvas();

            View.OpenCanvas<CanvasMaskView>();
        }

        private void OnLevelMasked()
        {
            Game.LevelBuilderManager.RestartLevel();

            View.OpenCanvas<CanvasMenuView>();
        }
    }
}