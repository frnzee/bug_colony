using R3;

namespace Stats
{
    public class ColonyStatsService : IColonyStatsService
    {
        private readonly ReactiveProperty<int> _deadWorkers = new(0);
        private readonly ReactiveProperty<int> _deadPredators = new(0);

        public ReadOnlyReactiveProperty<int> DeadWorkers => _deadWorkers;
        public ReadOnlyReactiveProperty<int> DeadPredators => _deadPredators;

        public void RegisterWorkerDeath() => _deadWorkers.Value++;
        public void RegisterPredatorDeath() => _deadPredators.Value++;
    }
}
