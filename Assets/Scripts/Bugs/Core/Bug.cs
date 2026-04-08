using System;
using Bugs.Movement;
using Bugs.States;
using Core;
using Gameplay.Services.GameTime;
using UnityEngine;
using Zenject;

namespace Bugs.Core
{
    public class Bug : MonoBehaviour, IBug
    {
        [SerializeField] private NavMeshBugMovement _movement;

        public IBugMovement Movement => _movement;
        public StateMachine StateMachine { get; } = new StateMachine();

        public bool IsAlive { get; private set; }
        public Vector3 Position => transform.position;
        public BugType Type => _behavior.BugType;

        public event Action<IBug> OnDied;

        private IBugBehavior _behavior;
        private ITicker _ticker;

        [Inject]
        private void Construct(ITicker ticker)
        {
            if (!_movement)
                _movement = gameObject.AddComponent<NavMeshBugMovement>();

            _ticker = ticker;
            _behavior = GetComponent<IBugBehavior>();

            if (!gameObject.activeInHierarchy) return;
            _ticker.Tick -= OnTick;
            _ticker.Tick += OnTick;
        }

        protected virtual void OnEnable()
        {
            IsAlive = true;
            if (_ticker == null) return;
            _ticker.Tick -= OnTick;
            _ticker.Tick += OnTick;
            _behavior?.OnSessionStart();
        }

        protected virtual void OnDisable()
        {
            if (_ticker == null) return;
            _ticker.Tick -= OnTick;
            _behavior?.OnSessionEnd();
        }

        public void BeEaten() => Kill();

        internal void Kill()
        {
            if (!IsAlive) return;
            IsAlive = false;
            gameObject.SetActive(false);
            OnDied?.Invoke(this);
            _behavior?.OnKilled();
        }

        private void OnTick(float deltaTime)
        {
            if (IsAlive)
                StateMachine.Tick(deltaTime);
        }

        public class WorkerFactory : PlaceholderFactory<Bug> { }
        public class PredatorFactory : PlaceholderFactory<Bug> { }
    }
}
