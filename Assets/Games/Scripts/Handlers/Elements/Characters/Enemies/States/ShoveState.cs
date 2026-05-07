using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class ShoveState : IEnemyState
    {
        private const float MaxChaseDistance = 5f;

        public void OnEnter(Enemy enemy)
        {
            enemy.PlayAnimation(GlobalValue.AnimationRun);
        }

        public void OnExecute(Enemy enemy)
        {
            if (ShouldAbortShove(enemy))
            {
                enemy.ChangeState<CollectState>();

                return;
            }

            if (AvoidState.ChangeToAvoidState(enemy))
            {
                return;
            }

            if (Game.Player != null)
            {
                enemy.SetDestination(Game.Player.transform.position);
            }
        }

        public void OnExit(Enemy enemy)
        {

        }

        private bool ShouldAbortShove(Enemy enemy)
        {
            if (Game.Player.BrickCount > enemy.BrickCount)
            {
                return true;
            }

            float distanceToPlayer = Vector3.Distance(enemy.transform.position, Game.Player.transform.position);
            if (distanceToPlayer > MaxChaseDistance)
            {
                return true;
            }

            return false;
        }
    }
}