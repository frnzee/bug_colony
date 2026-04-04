using System;
using System.Linq;
using Bugs.Movement;
using Colony;
using Core;
using Extensions;
using Resources;
using UnityEngine;

namespace Bugs.States
{
    public class PredatorWanderState : IBugState
    {
        private readonly IBugMovement _movement;
        private readonly Transform _transform;
        private readonly IColonyService _colonyService;
        private readonly IResourceRegistry _resourceRegistry;
        private readonly IBug _self;
        private readonly float _wanderRadius;
        private readonly float _changeInterval;
        private readonly float _detectionRadius;

        private float _timer;

        public event Action OnTargetDetected;

        public PredatorWanderState(
            IBugMovement movement,
            Transform transform,
            IColonyService colonyService,
            IResourceRegistry resourceRegistry,
            IBug self,
            float wanderRadius,
            float changeInterval,
            float detectionRadius)
        {
            _movement = movement;
            _transform = transform;
            _colonyService = colonyService;
            _resourceRegistry = resourceRegistry;
            _self = self;
            _wanderRadius = wanderRadius;
            _changeInterval = changeInterval;
            _detectionRadius = detectionRadius;
        }

        public void Enter()
        {
            _timer = _changeInterval;
        }

        public void Tick(float deltaTime)
        {
            if (HasNearbyTarget())
            {
                OnTargetDetected?.Invoke();
                return;
            }

            _timer += deltaTime;
            if (!(_timer >= _changeInterval) && !_movement.IsReachedDestination)
            {
                return;
            }
            
            SetRandomDestination();
            
            _timer = 0f;
        }

        public void Exit()
        {
        }

        private bool HasNearbyTarget()
        {
            var detectionSqr = _detectionRadius * _detectionRadius;

            return _colonyService.AliveBugs.Where(
                bug => bug.IsAlive && !ReferenceEquals(bug, _self))
                .Any(bug => (_transform.position - bug.Position).sqrMagnitude <= detectionSqr) || 
                _resourceRegistry.ActiveResources
                .Any(resource => resource.IsAlive && (_transform.position - resource.Position).sqrMagnitude <= detectionSqr);
        }

        private void SetRandomDestination()
        {
            var destination = _transform.position.GetRandomNavMeshPoint(_wanderRadius);
            _movement.SetDestination(destination);
        }
    }
}
