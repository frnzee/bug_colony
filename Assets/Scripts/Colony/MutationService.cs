using Bugs.Worker;
using UnityEngine;

namespace Colony
{
    public class MutationService : IMutationService
    {
        private readonly WorkerConfig _config;

        public MutationService(WorkerConfig config) => _config = config;

        public bool ShouldMutate(int colonySize) =>
            colonySize > _config.ColonySizeForMutation && Random.value < _config.MutationChance;
    }
}
