using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace YunaSpace.BridgeRace
{
    public class CanvasMenuView : ViewCanvas<CanvasMenuView>
    {
        [SerializeField] private Button startButton;
        [SerializeField] private TMP_InputField nameField;

        private void Awake()
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
            nameField.onValueChanged.AddListener(OnNameFieldValueChanged);
        }

        private void OnDestroy()
        {
            startButton.onClick.RemoveAllListeners();
            nameField.onValueChanged.RemoveAllListeners();
        }

        private void OnStartButtonClicked()
        {
            Close();

            View.OpenCanvas<CanvasCountdownView>();
        }

        private void OnNameFieldValueChanged(string value)
        {
            GlobalEvent.OnPlayerNameChanged?.Invoke(value);
        }
    }
}
