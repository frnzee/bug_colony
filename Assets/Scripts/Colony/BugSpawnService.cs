using Bugs.Predator;
using Bugs.Worker;
using Core;
using Infrastructure.Pool;
using UnityEngine;

namespace Colony
{
    public class BugSpawnService : IBugSpawnService
    {
        private readonly ObjectPool<WorkerBug> _workerPool;
        private readonly ObjectPool<PredatorBug> _predatorPool;
        private readonly IColonyService _colonyService;

        private readonly Transform _workerContainer;
        private readonly Transform _predatorContainer;

        public BugSpawnService(
            WorkerBug.Factory workerFactory,
            PredatorBug.Factory predatorFactory,
            IColonyService colonyService)
        {
            _colonyService = colonyService;
            _workerContainer = new GameObject("WorkerBugs").transform;
            _predatorContainer = new GameObject("PredatorBugs").transform;

            _workerPool = new ObjectPool<WorkerBug>(() => CreateWorker(workerFactory));
            _predatorPool = new ObjectPool<PredatorBug>(() => CreatePredator(predatorFactory));
        }

        public IBug SpawnWorker(Vector3 position)
        {
            var bug = _workerPool.Get(position);
            _colonyService.RegisterBug(bug);
            return bug;
        }

        public IBug SpawnPredator(Vector3 position)
        {
            var bug = _predatorPool.Get(position);
            _colonyService.RegisterBug(bug);
            return bug;
        }

        private WorkerBug CreateWorker(WorkerBug.Factory factory)
        {
            var bug = factory.Create();
            bug.transform.SetParent(_workerContainer);
            bug.OnDied += _ => _workerPool.Return(bug);
            return bug;
        }

        private PredatorBug CreatePredator(PredatorBug.Factory factory)
        {
            var bug = factory.Create();
            bug.transform.SetParent(_predatorContainer);
            bug.OnDied += _ => _predatorPool.Return(bug);
            return bug;
        }
    }
}
