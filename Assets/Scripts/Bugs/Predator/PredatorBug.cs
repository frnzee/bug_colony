using System;
using System.Threading;
using Bugs.Core;
using Bugs.Interaction;
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
    public class PredatorBehavior : MonoBehaviour, IBugBehavior
    {
        private const float SplitSpawnOffset = 5f;

        public BugType BugType => BugType.Predator;

        private PredatorConfig _config;
        private IBugSpawnService _spawnService;
        private IColonyStatsService _statsService;
        private IInteractionService _interactionService;

        private IBugMovement _movement;
        private StateMachine _stateMachine;

        private PredatorWanderState _wanderState;
        private SeekTargetState _seekTargetState;

        private Bug _bug;
        private int _feedCount;
        private bool _isSplitting;
        private CancellationTokenSource _lifetimeCts;

        [Inject]
        private void Construct(
            PredatorConfig config,
            IColonyService colonyService,
            IBugSpawnService spawnService,
            IColonyStatsService statsService,
            IInteractionService interactionService,
            IResourceRegistry resourceRegistry)
        {
            _config = config;
            _spawnService = spawnService;
            _statsService = statsService;
            _interactionService = interactionService;

            _bug = GetComponent<Bug>();
            _movement = _bug.Movement;
            _stateMachine = _bug.StateMachine;

            _movement.SetSpeed(_config.MoveSpeed);

            _wanderState = new PredatorWanderState(
                _movement, transform, colonyService, resourceRegistry, _bug,
                _config.WanderRadius, _config.WanderChangeInterval, _config.DetectionRadius);

            _seekTargetState = new SeekTargetState(
                _movement, transform, colonyService, resourceRegistry, _bug,
                _config.EatDistance, _interactionService);

            _wanderState.OnTargetDetected += () => _stateMachine.ChangeState(_seekTargetState);
            _seekTargetState.OnTargetEaten += HandleTargetEaten;
            _seekTargetState.OnNoTargetFound += () => _stateMachine.ChangeState(_wanderState);

            if (_bug.IsAlive)
                OnSessionStart();
        }

        public void OnSessionStart()
        {
            _feedCount = 0;
            _lifetimeCts?.Cancel();
            _lifetimeCts = new CancellationTokenSource();
            if (_wanderState != null)
                _stateMachine.ChangeState(_wanderState);
            StartLifetimeTimerAsync(_lifetimeCts.Token).Forget();
        }

        public void OnSessionEnd() => _lifetimeCts?.Cancel();

        public void OnKilled()
        {
            _lifetimeCts?.Cancel();
            if (!_isSplitting)
                _statsService?.RegisterPredatorDeath();
        }

        private void HandleTargetEaten()
        {
            ++_feedCount;
            if (_feedCount >= _config.FeedCountToSplit)
                Split();
            else
                _stateMachine.ChangeState(_wanderState);
        }

        private void Split()
        {
            var spawnPos = transform.position;
            var dir = Random.insideUnitSphere;
            var offsetDir = new Vector3(dir.x, 0f, dir.z).normalized * SplitSpawnOffset;

            _spawnService.Spawn(BugType.Predator, (spawnPos + offsetDir).SampleNavMesh(SplitSpawnOffset));
            _spawnService.Spawn(BugType.Predator, (spawnPos - offsetDir).SampleNavMesh(SplitSpawnOffset));

            _isSplitting = true;
            _bug.Kill();
            _isSplitting = false;
        }

        private async UniTaskVoid StartLifetimeTimerAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_config.LifetimeDuration), cancellationToken: token);
            _bug.Kill();
        }
    }
}
