using System;
using System.Threading;
using Bugs.Core;
using Bugs.Movement;
using Bugs.States;
using Colony;
using Core;
using Cysharp.Threading.Tasks;
using Extensions;
using Resources;
using Stats;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Bugs.Predator
{
    public class PredatorBug : BugBase
    {
        public override BugType Type => BugType.Predator;

        [SerializeField] private NavMeshBugMovement _movement;

        private PredatorConfig _config;
        private IBugSpawnService _spawnService;
        private IColonyStatsService _statsService;
        private CancellationTokenSource _lifetimeCts;
        private PredatorWanderState _wanderState;
        private SeekTargetState _seekTargetState;

        private int _feedCount;
        private bool _isSplitting;
        
        [Inject]
        private void Construct(
            PredatorConfig config,
            IColonyService colonyService,
            IBugSpawnService spawnService,
            IColonyStatsService statsService,
            IResourceRegistry resourceRegistry)
        {
            _config = config;
            _spawnService = spawnService;
            _statsService = statsService;

            if (!_movement)
            {
                _movement = gameObject.AddComponent<NavMeshBugMovement>();
            }

            _movement.SetSpeed(_config.MoveSpeed);

            _wanderState = new PredatorWanderState(
                _movement, transform, colonyService, resourceRegistry, this,
                _config.WanderRadius, _config.WanderChangeInterval, _config.DetectionRadius);

            _seekTargetState = new SeekTargetState(
                _movement, transform, colonyService, resourceRegistry, this, _config.EatDistance);

            _wanderState.OnTargetDetected += () => StateMachine.ChangeState(_seekTargetState);
            _seekTargetState.OnTargetEaten += HandleTargetEaten;
            _seekTargetState.OnNoTargetFound += () => StateMachine.ChangeState(_wanderState);

            StartSession();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_wanderState != null)
            {
                StartSession();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            _lifetimeCts?.Cancel();
        }

        protected override void OnKilled()
        {
            _lifetimeCts?.Cancel();
            
            if (!_isSplitting)
            {
                _statsService?.RegisterPredatorDeath();
            }
        }

        private void StartSession()
        {
            _feedCount = 0;
            _lifetimeCts?.Cancel();
            _lifetimeCts = new CancellationTokenSource();
            StateMachine.ChangeState(_wanderState);
            StartLifetimeTimerAsync(_lifetimeCts.Token).Forget();
        }

        private void HandleTargetEaten()
        {
            ++_feedCount;

            if (_feedCount >= _config.FeedCountToSplit)
            {
                Split();
            }
            else
            {
                StateMachine.ChangeState(_wanderState);
            }
        }

        private const float SplitSpawnOffset = 5f;

        private void Split()
        {
            var spawnPos = transform.position;
            var dir = Random.insideUnitSphere;
            var offsetDir = new Vector3(dir.x, 0f, dir.z).normalized * SplitSpawnOffset;

            _spawnService.SpawnPredator((spawnPos + offsetDir).SampleNavMesh(SplitSpawnOffset));
            _spawnService.SpawnPredator((spawnPos - offsetDir).SampleNavMesh(SplitSpawnOffset));

            _isSplitting = true;
            
            Kill();
            
            _isSplitting = false;
        }

        private async UniTaskVoid StartLifetimeTimerAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_config.LifetimeDuration), cancellationToken: token);
            
            Kill();
        }

        public class Factory : PlaceholderFactory<PredatorBug> { }
    }
}
