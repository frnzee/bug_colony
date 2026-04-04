using R3;

namespace Stats
{
    public interface IColonyStatsService
    {
        ReadOnlyReactiveProperty<int> DeadWorkers { get; }
        ReadOnlyReactiveProperty<int> DeadPredators { get; }
        void RegisterWorkerDeath();
        void RegisterPredatorDeath();
    }
}
