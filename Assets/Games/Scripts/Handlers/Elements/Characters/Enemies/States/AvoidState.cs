
using UnityEngine;
using UnityEngine.AI;

namespace YunaSpace.BridgeRace
{
    public class AvoidState : IEnemyState
    {
        private const float AvoidDuration = 3f;
        private const float FleeDistance = 5f;

        private float avoidTimer;

        public void OnEnter(Enemy enemy)
        {
            enemy.PlayAnimation(GlobalValue.AnimationRun);

            avoidTimer = 0;

            CalculateFleePath(enemy);
        }

        public void OnExecute(Enemy enemy)
        {
            avoidTimer += Time.deltaTime;

            if (avoidTimer >= AvoidDuration)
            {
                enemy.ChangeState<CollectState>();
                return;
            }

            if (Vector3.Distance(enemy.transform.position, Game.Player.transform.position) < 3f)
            {
                CalculateFleePath(enemy);
            }
        }

        public void OnExit(Enemy enemy)
        {

        }

        public static bool ChangeToAvoidState(Enemy enemy)
        {
            float distanceToPlayer = Vector3.Distance(enemy.transform.position, Game.Player.transform.position);

            if (distanceToPlayer < GlobalValue.StateAvoidDistance && Game.Player.BrickCount > enemy.BrickCount && enemy.BrickCount > 0)
            {
                enemy.ChangeState<AvoidState>();

                return true;
            }

            return false;
        }

        private void CalculateFleePath(Enemy enemy)
        {
            var fleeDirection = (enemy.transform.position - Game.Player.transform.position).normalized;

            var targetPos = enemy.transform.position + fleeDirection * FleeDistance;

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, FleeDistance, NavMesh.AllAreas) && enemy.Agent.enabled)   
            {
                enemy.SetDestination(hit.position);
            }
        }
    }
}