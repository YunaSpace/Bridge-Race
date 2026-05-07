using System;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public static class GlobalEvent
    {
        public static Action<int> OnLevelLoaded { get; set; }
        public static Action OnLevelInitialized { get; set; }
        public static Action OnLevelStarted { get; set; }
        public static Action OnLevelFinished { get; set; }
        public static Action OnLevelNexted { get; set; }
        public static Action OnLevelCleared { get; set; }
        public static Action OnLevelPaused { get; set; }
        public static Action OnLevelResumed { get; set; }
        public static Action OnLevelMasked { get; set; }

        public static Action<float, float> OnJoystickMoved { get; set; }
        public static Action<string> OnPlayerNameChanged { get; set; }
    }
}