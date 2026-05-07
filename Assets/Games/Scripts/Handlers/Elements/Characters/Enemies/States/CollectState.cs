using UnityEngine;

namespace YunaSpace.BridgeRace
{
    public class CollectState : IEnemyState
    {
        private const float ShoveTimeThreshold = 5f;
        private const float ShoveDistanceThreshold = 2f;

        private GroundBrick targetBrick;

        private float shoveTimer;
        private bool readyToShove;

        private int targetBrickToCollect;

        public void OnEnter(Enemy enemy)
        {
            shoveTimer = 0;
            readyToShove = false;

            targetBrickToCollect = Random.Range(5, GlobalValue.MaxBrickCarried - 5);

            enemy.PlayAnimation(GlobalValue.AnimationRun);

            FindNextBrick(enemy);
        }

        public void OnExecute(Enemy enemy)
        {
            shoveTimer += Time.deltaTime;

            HandleShoveLogic(enemy);
            HandleCollectionLogic(enemy);
            HandleStateTransitions(enemy);
        }

        public void OnExit(Enemy enemy)
        {
        }

        private void HandleShoveLogic(Enemy  enemy)
        {
            if (AvoidState.ChangeToAvoidState(enemy))
            {
                return;
            }

            if (shoveTimer >= ShoveTimeThreshold)
            {
                var currentBrickCount = enemy.BrickCount;
                var playerBrickCount = Game.Player.BrickCount;

                readyToShove = currentBrickCount > playerBrickCount;
            }

            float distanceToPlayer = Vector3.Distance(enemy.transform.position, Game.Player.transform.position);

            if (readyToShove)
            {
                if (distanceToPlayer <= ShoveDistanceThreshold)
                {
                    enemy.ChangeState<ShoveState>();

                    return;
                }
            }
        }

        private void HandleCollectionLogic(Enemy enemy)
        {
            if (targetBrick == null || !targetBrick.gameObject.activeInHierarchy)
            {
                FindNextBrick(enemy);
            }
            else
            {
                if (enemy.Agent.pathPending == false && enemy.Agent.remainingDistance <= enemy.Agent.stoppingDistance && enemy.Agent.enabled)
                {
                    FindNextBrick(enemy);
                }
            }
        }

        private void HandleStateTransitions(Enemy enemy)
        {
            if (enemy.BrickCount >= targetBrickToCollect)
            {
                enemy.ChangeState<BridgeState>();
            }
        }

        private void FindNextBrick(Enemy enemy)
        {
            if (enemy.CurrentStage >= Game.LevelBuilderManager.MaxStage)
            {
                enemy.ChangeState<GoalState>();
                return;
            }

            targetBrick = Game.StageGroundManager.FindNearestBrick(enemy.transform.position, enemy.ColorType, enemy.CurrentStage);

            if (targetBrick != null)
            {
                if (enemy.Agent.enabled && enemy.IsOnGround)
                {
                    enemy.SetDestination(targetBrick.transform.position);
                }

                enemy.PlayAnimation(GlobalValue.AnimationRun);
            }
            else
            {
                enemy.PlayAnimation(GlobalValue.AnimationIdle);
            }
        }
    }
}