using System;
using System.Linq;
using Bugs.Movement;
using Extensions;
using Resources;
using UnityEngine;

namespace Bugs.States
{
    public class WanderState : IBugState
    {
        private readonly IBugMovement _movement;
        private readonly Transform _transform;
        private readonly IResourceRegistry _resourceRegistry;
        private readonly float _wanderRadius;
        private readonly float _changeInterval;
        private readonly float _detectionRadius;

        private float _timer;

        public event Action OnFoodDetected;

        public WanderState(
            IBugMovement movement,
            Transform transform,
            IResourceRegistry resourceRegistry,
            float wanderRadius,
            float changeInterval,
            float detectionRadius)
        {
            _movement = movement;
            _transform = transform;
            _resourceRegistry = resourceRegistry;
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
            if (HasNearbyFood())
            {
                OnFoodDetected?.Invoke();
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

        private bool HasNearbyFood()
        {
            var detectionSqr = _detectionRadius * _detectionRadius;
            return _resourceRegistry.ActiveResources.Any(resource => resource.IsAlive && (_transform.position - resource.Position).sqrMagnitude <= detectionSqr);
        }

        private void SetRandomDestination()
        {
            var destination = _transform.position.GetRandomNavMeshPoint(_wanderRadius);
            _movement.SetDestination(destination);
        }
    }
}
