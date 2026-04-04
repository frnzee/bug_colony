using System;
using Bugs.Movement;
using Resources;
using UnityEngine;

namespace Bugs.States
{
    public class SeekResourceState : IBugState
    {
        private readonly IBugMovement _movement;
        private readonly Transform _transform;
        private readonly IResourceRegistry _resourceRegistry;
        private readonly float _eatDistance;

        private IResource _targetResource;

        public event Action OnResourceEaten;
        public event Action OnResourceLost;

        public SeekResourceState(
            IBugMovement movement,
            Transform transform,
            IResourceRegistry resourceRegistry,
            float eatDistance)
        {
            _movement = movement;
            _transform = transform;
            _resourceRegistry = resourceRegistry;
            _eatDistance = eatDistance;
        }

        public void Enter()
        {
            _targetResource = FindNearestResource();
            
            if (_targetResource != null)
            {
                _movement.SetDestination(_targetResource.Position);
            }
            else
            {
                OnResourceLost?.Invoke();
            }
        }

        public void Tick(float deltaTime)
        {
            if (_targetResource is not { IsAlive: true })
            {
                OnResourceLost?.Invoke();
                return;
            }

            var distanceSqr = (_transform.position - _targetResource.Position).sqrMagnitude;
            
            if (!(distanceSqr <= _eatDistance * _eatDistance))
            {
                return;
            }
            
            _targetResource.BeEaten();
            OnResourceEaten?.Invoke();
        }

        public void Exit()
        {
            _movement.Stop();
            _targetResource = null;
        }

        private IResource FindNearestResource()
        {
            IResource nearest = null;
            var nearestSqrDist = float.MaxValue;

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
