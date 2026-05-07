using UnityEngine;
using UnityEngine.AI;

namespace YunaSpace.BridgeRace
{
    public class Player : Character
    {
        public PlayerMovementHandler MovementHandler;

        protected override void Awake()
        {
            base.Awake();

            GlobalEvent.OnPlayerNameChanged += OnPlayerNameChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            GlobalEvent.OnPlayerNameChanged -= OnPlayerNameChanged;
        }

        public override void OnInitialize()
        {
            brickCount = 0;
            totalBrickCount = 0;
            currentStage = 0;

            BrickHandler.OnInitialize();
        }

        public override void OnStart()
        {

        }

        protected override void OnPause()
        {
            base.OnPause();

            Physics.simulationMode = SimulationMode.Script;
        }

        protected override void OnResume()
        {
            base.OnResume();

            Physics.simulationMode = SimulationMode.FixedUpdate;
        }

        public override void OnFullBridgeReached(int nextStage)
        {
            base.OnFullBridgeReached(nextStage);
        }

        public override void OnBrickCountChanged()
        {
            Game.CameraFocus.UpdateFocus(BrickCount);
        }

        public override void Stumble(Vector3 direction)
        {
            base.Stumble(direction);

            PlayAnimation(GlobalValue.AnimationFall1);
        }

        protected override void ResetStumble()
        {
            if (isPaused)
            {
                return;
            }

            base.ResetStumble();

            PlayAnimation(GlobalValue.AnimationIdle);
        }
    
        private void OnPlayerNameChanged(string name)
        {
            nameBadge.text = name;
        }
    }
}