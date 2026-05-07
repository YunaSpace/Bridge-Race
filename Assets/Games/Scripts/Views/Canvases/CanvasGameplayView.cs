using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace YunaSpace.BridgeRace
{
    public class CanvasGameplayView : ViewCanvas<CanvasGameplayView>
    {
        [SerializeField] private Button settingsButton;
        [SerializeField] private TextMeshProUGUI levelText;

        private void Awake()
        {
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        }

        private void OnDestroy()
        {
            settingsButton.onClick.RemoveAllListeners();
        }

        public override void OnOpened()
        {
            base.OnOpened();

            levelText.text = $"Level {Game.LevelBuilderManager.CurrentLevel}";
        }

        private void OnSettingsButtonClicked()
        {
            GlobalEvent.OnLevelPaused?.Invoke();

            View.OpenCanvas<CanvasSettingsView>();
        }
    }
}
