using TMPro;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class Enemy : Character
    {
        public Bridge CurrentBridge => currentBridge;

        public BridgeChecker BridgeChecker => bridgeChecker;

        [SerializeField] private EnemyStateHandler stateHandler;

        [SerializeField] private Bridge currentBridge;
        [SerializeField] private BridgeChecker bridgeChecker;

        public override void OnStart()
        {
            ChangeState<CollectState>();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        public override void OnFullBridgeReached(int nextStage)
        {
            base.OnFullBridgeReached(nextStage);

            SetBridge(null);

            ChangeState<CollectState>();
        }

        public void ChangeState<T>() where T : IEnemyState => stateHandler.ChangeState<T>();

        public void SetBridge(Bridge bridge) => currentBridge = bridge;

        public void SetBridgeChecker(BridgeChecker bridgeChecker) => this.bridgeChecker = bridgeChecker;

        public void ChangeNameBadge(string name)
        {
            nameBadge.SetText(name);
        }

        public override void Stumble(Vector3 direction)
        {
            if (isStumbling) return;

            base.Stumble(direction);
            
            ChangeState<StumbleState>();
        }

        protected override void ResetStumble()
        {
            if (isPaused)
            {
                return;
            }

            isStumbling = false;

            base.ResetStumble();

            ChangeState<CollectState>();
        }
    }
}