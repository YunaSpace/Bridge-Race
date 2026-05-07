namespace YunaSpace.BridgeRace
{
    public interface IEnemyState
    {
        void OnEnter(Enemy enemy);
        void OnExecute(Enemy enemy);
        void OnExit(Enemy enemy);
    }
}