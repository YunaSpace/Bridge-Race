using UnityEngine;
using UnityEngine.UI;

namespace YunaSpace.BridgeRace
{
    public class CanvasVictoryView : ViewCanvas<CanvasVictoryView>
    {
        [SerializeField] private Button nextButton;
        [SerializeField] private Button menuButton;

        private void Awake()
        {
            nextButton.onClick.AddListener(OnNextButtonClicked);
            menuButton.onClick.AddListener(OnMenuButtonClicked);

            GlobalEvent.OnLevelMasked += OnLevelMasked;
        }

        private void OnDestroy()
        {
            nextButton.onClick.RemoveAllListeners();
            menuButton.onClick.RemoveAllListeners();

            GlobalEvent.OnLevelMasked -= OnLevelMasked;
        }

        private void OnNextButtonClicked()
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
            Game.LevelBuilderManager.NextLevel();

            View.OpenCanvas<CanvasMenuView>();
        }
    }
}