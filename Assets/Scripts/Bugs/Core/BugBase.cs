using System;
using Bugs.States;
using Core;
using Gameplay.Services.GameTime;
using UnityEngine;
using Zenject;

namespace Bugs.Core
{
    public abstract class BugBase : MonoBehaviour, IBug
    {
        public abstract BugType Type { get; }
        public Vector3 Position => transform.position;
        public event Action<IBug> OnDied;
        public bool IsAlive { get; private set; } = true;
        
        protected readonly StateMachine StateMachine = new();
        
        private ITicker _ticker;

        [Inject]
        private void ConstructTicker(ITicker ticker)
        {
            _ticker = ticker;
            _ticker.Tick += OnTick;
        }

        protected virtual void OnEnable()
        {
            IsAlive = true;

            if (_ticker != null)
            {
                _ticker.Tick += OnTick;
            }
        }

        protected virtual void OnDisable()
        {
            if (_ticker != null)
            {
                _ticker.Tick -= OnTick;
            }
        }

        public void BeEaten() => Kill();

        protected void Kill()
        {
            if (!IsAlive)
            {
                return;
            }

            IsAlive = false;
            gameObject.SetActive(false);
            OnDied?.Invoke(this);
            OnKilled();
        }

        protected abstract void OnKilled();

        private void OnTick(float deltaTime)
        {
            if (IsAlive)
            {
                StateMachine.Tick(deltaTime);
            }
        }
    }
}
