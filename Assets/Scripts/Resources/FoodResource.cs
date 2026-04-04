using System;
using UnityEngine;
using Zenject;

namespace Resources
{
    public class FoodResource : MonoBehaviour, IResource
    {
        private IResourceRegistry _registry;

        public bool IsAlive { get; private set; } = true;

        public Vector3 Position => transform.position;

        public event Action<FoodResource> OnConsumed;

        [Inject]
        private void Construct(IResourceRegistry registry)
        {
            _registry = registry;
        }

        private void OnEnable()
        {
            IsAlive = true;
            _registry?.Register(this);
        }

        private void OnDisable()
        {
            _registry?.Unregister(this);
        }

        public void BeEaten()
        {
            if (!IsAlive)
            {
                return;
            }

            IsAlive = false;
            OnConsumed?.Invoke(this);
            gameObject.SetActive(false);
        }

        public class Factory : PlaceholderFactory<FoodResource> { }
    }
}
