using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class GoalState : IEnemyState
    {
        public void OnEnter(Enemy enemy)
        {
            enemy.PlayAnimation(GlobalValue.AnimationRun);

            GlobalEvent.OnLevelFinished?.Invoke();
        }

        public void OnExecute(Enemy enemy)
        {

        }

        public void OnExit(Enemy enemy)
        {

        }
    }
}