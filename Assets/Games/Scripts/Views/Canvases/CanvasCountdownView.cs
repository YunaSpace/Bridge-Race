using TMPro;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    internal class CanvasCountdownView : ViewCanvas<CanvasCountdownView>
    {
        [SerializeField] private TextMeshProUGUI counterText;

        private float countdownTime = 3f;
        private int ceiledTime;

        private void Update()
        {
            countdownTime -= Time.deltaTime;

            if (countdownTime < 0)
            {
                Close();

                View.OpenCanvas<CanvasJoystickView>();
                View.OpenCanvas<CanvasGameplayView>();

                if (GlobalValue.IsLevelStarted == false)
                {
                    GlobalEvent.OnLevelStarted?.Invoke();

                    GlobalValue.IsLevelStarted = true;
                }
                else
                {
                    GlobalEvent.OnLevelResumed?.Invoke();
                }
            }

            int currentCeiledTime = (int)Mathf.Ceil(countdownTime);
            float realPart = currentCeiledTime - countdownTime;

            counterText.rectTransform.localScale = Vector3.one * Mathf.Lerp(1.5f, 0.5f, realPart);

            if (ceiledTime != currentCeiledTime)
            {
                counterText.text = $"{currentCeiledTime}";
                ceiledTime = currentCeiledTime;
            }
        }

        public override void OnOpened()
        {
            base.OnOpened();

            countdownTime = 3;
        }
    }
}
