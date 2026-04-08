using System;
using Bugs.Interaction;
using Bugs.Movement;
using Colony;
using Core;
using Resources;
using UnityEngine;

namespace Bugs.States
{
    public class SeekTargetState : IBugState
    {
        private readonly IBugMovement _movement;
        private readonly Transform _transform;
        private readonly IColonyService _colonyService;
        private readonly IResourceRegistry _resourceRegistry;
        private readonly IBug _self;
        private readonly float _eatDistance;
        private readonly IInteractionService _interactionService;

        private IEatable _currentTarget;
        private float _pathUpdateTimer;
        private const float PathUpdateInterval = 0.25f;

        public event Action OnTargetEaten;
        public event Action OnNoTargetFound;

        public SeekTargetState(
            IBugMovement movement,
            Transform transform,
            IColonyService colonyService,
            IResourceRegistry resourceRegistry,
            IBug self,
            float eatDistance,
            IInteractionService interactionService)
        {
            _movement = movement;
            _transform = transform;
            _colonyService = colonyService;
            _resourceRegistry = resourceRegistry;
            _self = self;
            _eatDistance = eatDistance;
            _interactionService = interactionService;
        }

        public void Enter()
        {
            _pathUpdateTimer = PathUpdateInterval;
            RefreshTarget();
        }

        public void Tick(float deltaTime)
        {
            if (_currentTarget == null || !_currentTarget.IsAlive)
            {
                RefreshTarget();
                return;
            }

            _pathUpdateTimer += deltaTime;
            if (_pathUpdateTimer >= PathUpdateInterval)
            {
                _movement.SetDestination(_currentTarget.Position);
                _pathUpdateTimer = 0f;
            }

            if ((_transform.position - _currentTarget.Position).sqrMagnitude > _eatDistance * _eatDistance)
                return;

            _interactionService.Interact(_currentTarget);
            OnTargetEaten?.Invoke();
        }

        public void Exit()
        {
            _movement.Stop();
            _currentTarget = null;
        }

        private void RefreshTarget()
        {
            _currentTarget = FindNearestTarget();
            if (_currentTarget != null)
                _movement.SetDestination(_currentTarget.Position);
            else
                OnNoTargetFound?.Invoke();
        }

        private IEatable FindNearestTarget()
        {
            var nearestBug = FindNearestBug();
            var nearestResource = FindNearestResource();

            if (nearestBug == null) return nearestResource;
            if (nearestResource == null) return nearestBug;

            var bugSqr = (_transform.position - nearestBug.Position).sqrMagnitude;
            var resSqr = (_transform.position - nearestResource.Position).sqrMagnitude;
            return bugSqr < resSqr ? nearestBug : nearestResource;
        }

        private IEatable FindNearestBug()
        {
            IEatable nearest = null;
            var nearestSqrDist = float.MaxValue;

            foreach (var bug in _colonyService.AliveBugs)
            {
                if (!bug.IsAlive || ReferenceEquals(bug, _self) || !_interactionService.CanInteract(_self, bug))
                    continue;
                var sqrDist = (_transform.position - bug.Position).sqrMagnitude;
                if (sqrDist >= nearestSqrDist) continue;
                nearestSqrDist = sqrDist;
                nearest = bug;
            }

            return nearest;
        }

        private IEatable FindNearestResource()
        {
            IEatable nearest = null;
            var nearestSqrDist = float.MaxValue;

            foreach (var resource in _resourceRegistry.ActiveResources)
            {
                if (!resource.IsAlive) continue;
                var sqrDist = (_transform.position - resource.Position).sqrMagnitude;
                if (sqrDist >= nearestSqrDist) continue;
                nearestSqrDist = sqrDist;
                nearest = resource;
            }

            return nearest;
        }
    }
}
