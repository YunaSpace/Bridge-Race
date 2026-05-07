namespace YunaSpace.BridgeRace
{
    public class StumbleState : IEnemyState
    {
        public void OnEnter(Enemy enemy)
        {
            enemy.PlayAnimation(GlobalValue.AnimationFall1);

            if (enemy.IsOnGround)
            {
                enemy.SetMovable(false);
            }

            enemy.Rigidbody.isKinematic = false;
        }

        public void OnExecute(Enemy enemy)
        {

        }

        public void OnExit(Enemy enemy)
        {
            if (enemy.IsOnGround)
            {
                enemy.SetMovable(true);
            }

            enemy.Rigidbody.isKinematic = true;
        }
    }
}