using System.Collections.Generic;
using Bugs.Core;
using Core;
using Infrastructure.Pool;
using UnityEngine;
using Zenject;

namespace Colony
{
    public class BugSpawnService : IBugSpawnService
    {
        private readonly Dictionary<BugType, ObjectPool<Bug>> _pools = new Dictionary<BugType, ObjectPool<Bug>>();
        private readonly IColonyService _colonyService;

        public BugSpawnService(
            Bug.WorkerFactory workerFactory,
            Bug.PredatorFactory predatorFactory,
            IColonyService colonyService)
        {
            _colonyService = colonyService;

            var workerContainer = new GameObject("WorkerBugs").transform;
            var predatorContainer = new GameObject("PredatorBugs").transform;

            _pools[BugType.Worker] = new ObjectPool<Bug>(() => CreateBug(workerFactory, workerContainer));
            _pools[BugType.Predator] = new ObjectPool<Bug>(() => CreateBug(predatorFactory, predatorContainer));
        }

        public IBug Spawn(BugType type, Vector3 position)
        {
            var bug = _pools[type].Get(position);
            _colonyService.RegisterBug(bug);
            return bug;
        }

        private Bug CreateBug(PlaceholderFactory<Bug> factory, Transform container)
        {
            var bug = factory.Create();
            bug.transform.SetParent(container);
            bug.OnDied += _ => _pools[bug.Type].Return(bug);
            return bug;
        }
    }
}
