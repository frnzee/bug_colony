using Bugs.Core;
using Bugs.Movement;
using Bugs.States;
using Colony;
using Core;
using Extensions;
using Resources;
using Stats;
using UnityEngine;
using Zenject;

namespace Bugs.Worker
{
    public class WorkerBug : BugBase
    {
        private const float SplitSpawnOffset = 1f;

        public override BugType Type => BugType.Worker;

        [SerializeField] private NavMeshBugMovement _movement;

        private WorkerConfig _config;
        private IColonyService _colonyService;
        private IBugSpawnService _spawnService;
        private IColonyStatsService _statsService;
        private WanderState _wanderState;
        private SeekResourceState _seekResourceState;

        private int _feedCount;
        private bool _isSplitting;
        
        [Inject]
        private void Construct(
            WorkerConfig config,
            IColonyService colonyService,
            IBugSpawnService spawnService,
            IColonyStatsService statsService,
            IResourceRegistry resourceRegistry)
        {
            _config = config;
            _colonyService = colonyService;
            _spawnService = spawnService;
            _statsService = statsService;

            if (!_movement)
            {
                _movement = gameObject.AddComponent<NavMeshBugMovement>();
            }

            _movement.SetSpeed(_config.MoveSpeed);

            _wanderState = new WanderState(_movement, transform, resourceRegistry, _config.WanderRadius, _config.WanderChangeInterval, _config.DetectionRadius);

            _seekResourceState = new SeekResourceState(_movement, transform, resourceRegistry, _config.EatDistance);

            _wanderState.OnFoodDetected += () => StateMachine.ChangeState(_seekResourceState);
            _seekResourceState.OnResourceEaten += HandleResourceEaten;
            _seekResourceState.OnResourceLost += () => StateMachine.ChangeState(_wanderState);

            StateMachine.ChangeState(_wanderState);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _feedCount = 0;

            if (_wanderState != null)
            {
                StateMachine.ChangeState(_wanderState);
            }
        }

        protected override void OnKilled()
        {
            if (!_isSplitting)
            {
                _statsService?.RegisterWorkerDeath();
            }
        }

        private void HandleResourceEaten()
        {
            _feedCount++;

            if (_feedCount >= _config.FeedCountToSplit)
            {
                Split();
            }
            else
            {
                StateMachine.ChangeState(_wanderState);
            }
        }
        
        private void Split()
        {
            var spawnPos = transform.position;
            var dir = Random.insideUnitSphere;
            var offsetDir = new Vector3(dir.x, 0f, dir.z).normalized * SplitSpawnOffset;

            _spawnService.SpawnWorker((spawnPos + offsetDir).SampleNavMesh(SplitSpawnOffset));
            SpawnSecondOffspring((spawnPos - offsetDir).SampleNavMesh(SplitSpawnOffset));

            _isSplitting = true;
            Kill();
            _isSplitting = false;
        }

        private void SpawnSecondOffspring(Vector3 pos)
        {
            var shouldMutate = _colonyService.AliveBugCount - 1 > _config.ColonySizeForMutation && Random.value < _config.MutationChance;

            if (shouldMutate)
            {
                _spawnService.SpawnPredator(pos);
            }
            else
            {
                _spawnService.SpawnWorker(pos);
            }
        }

        public class Factory : PlaceholderFactory<WorkerBug> { }
    }
}
