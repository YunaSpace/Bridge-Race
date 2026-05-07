using Unity.VisualScripting;
using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class BridgeState : IEnemyState
    {
        private bool isTargetingBridge;

        public void OnEnter(Enemy enemy)
        {
            enemy.PlayAnimation(GlobalValue.AnimationRun);

            var bridge = GetTargetBridge(enemy);

            if (bridge != null)
            {
                enemy.SetBridge(bridge);
                enemy.SetDestination(bridge.transform.position);

                isTargetingBridge = true;
            }

            enemy.SetSpeed(5);
        }

        public void OnExecute(Enemy enemy)
        {
            if (isTargetingBridge && enemy.RemainingDistance < 1)
            {
                var bridgeChecker = enemy.CurrentBridge.GetBridgeChecker(enemy.ColorType);
                
                enemy.SetBridgeChecker(bridgeChecker);

                isTargetingBridge = false;
            }

            if (enemy.BridgeChecker)
            {
                enemy.SetDestination(enemy.BridgeChecker.transform.position);
            }

            if (enemy.BrickCount < 1)
            {
                enemy.ChangeState<CollectState>();
            }
        }

        public void OnExit(Enemy enemy)
        {
            enemy.SetBridgeChecker(null);

            enemy.SetSpeed(7.5f);
        }

        private Bridge GetTargetBridge(Enemy enemy)
        {
            Bridge currentBridge = enemy.CurrentBridge;

            if (currentBridge != null && currentBridge.GetStairAmount(enemy.ColorType) > 0 && currentBridge.StartColor == enemy.ColorType)
            {
                return currentBridge;
            }

            var emptyBridge = Game.StageBridgeManager.GetEmptyBridge(enemy.CurrentStage);

            if (emptyBridge != null)
            {
                return emptyBridge;
            }

            var fullBridge = Game.StageBridgeManager.GetRandomFullBridge(enemy.CurrentStage);

            if (fullBridge != null)
            {
                return fullBridge;
            }

            var fewestBridge = Game.StageBridgeManager.GetFewestColoredBridgeExcept(enemy.ColorType, enemy.CurrentStage);

            return fewestBridge;
        }
    }
}