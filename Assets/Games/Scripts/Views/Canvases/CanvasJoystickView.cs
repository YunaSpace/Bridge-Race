using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class CanvasJoystickView : ViewCanvas<CanvasJoystickView>
    {
        [SerializeField] private FloatingJoystick joystick;

        private void Awake()
        {
            joystick.OnValueChanged.AddListener(OnJoystickValueChanged);
        }

        private void OnDestroy()
        {
            joystick.OnValueChanged.RemoveAllListeners();
        }

        private void OnJoystickValueChanged(float horizontal, float vertical)
        {
            GlobalEvent.OnJoystickMoved?.Invoke(horizontal, vertical);
        }
    }
}
