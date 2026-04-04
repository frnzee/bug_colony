using System;
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
            float eatDistance)
        {
            _movement = movement;
            _transform = transform;
            _colonyService = colonyService;
            _resourceRegistry = resourceRegistry;
            _self = self;
            _eatDistance = eatDistance;
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

            var distanceSqr = (_transform.position - _currentTarget.Position).sqrMagnitude;
            
            if (!(distanceSqr <= _eatDistance * _eatDistance))
            {
                return;
            }
            
            _currentTarget.BeEaten();
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
            {
                _movement.SetDestination(_currentTarget.Position);
            }
            else
            {
                OnNoTargetFound?.Invoke();
            }
        }

        private IEatable FindNearestTarget()
        {
            IEatable nearest = null;
            
            var nearestSqrDist = float.MaxValue;

            foreach (var bug in _colonyService.AliveBugs)
            {
                if (!bug.IsAlive || ReferenceEquals(bug, _self))
                    continue;

                var sqrDist = (_transform.position - bug.Position).sqrMagnitude;
                
                if (!(sqrDist < nearestSqrDist))
                {
                    continue;
                }
                
                nearestSqrDist = sqrDist;
                nearest = bug;
            }

            foreach (var resource in _resourceRegistry.ActiveResources)
            {
                if (!resource.IsAlive)
                    continue;

                var sqrDist = (_transform.position - resource.Position).sqrMagnitude;
                
                if (!(sqrDist < nearestSqrDist))
                {
                    continue;
                }
                
                nearestSqrDist = sqrDist;
                nearest = resource;
            }

            return nearest;
        }
    }
}
