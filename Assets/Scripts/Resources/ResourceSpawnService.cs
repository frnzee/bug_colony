using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extensions;
using Infrastructure.Pool;
using UnityEngine;

namespace Resources
{
    public class ResourceSpawnService : IDisposable
    {
        private readonly ObjectPool<FoodResource> _foodPool;
        private readonly IResourceRegistry _registry;
        private readonly ResourceConfig _config;
        private readonly Transform _foodContainer;

        private CancellationTokenSource _cts;

        public ResourceSpawnService(
            FoodResource.Factory foodFactory,
            IResourceRegistry registry,
            ResourceConfig config)
        {
            _registry = registry;
            _config = config;
            _foodContainer = new GameObject("FoodResources").transform;
            _foodPool = new ObjectPool<FoodResource>(() => CreateFood(foodFactory));
        }

        public void StartSpawning()
        {
            _cts = new CancellationTokenSource();
            SpawnLoopAsync(_cts.Token).Forget();
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        private async UniTaskVoid SpawnLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_config.SpawnInterval), cancellationToken: token);

                if (_registry.ActiveResources.Count < _config.MaxResourcesOnScene)
                {
                    SpawnFood();
                }
            }
        }

        private void SpawnFood()
        {
            var position = Vector3.zero.GetRandomNavMeshPoint(_config.SpawnAreaRadius);
            _foodPool.Get(position);
        }

        private FoodResource CreateFood(FoodResource.Factory factory)
        {
            var food = factory.Create();
            food.transform.SetParent(_foodContainer);
            food.OnConsumed += f => _foodPool.Return(f);
            return food;
        }
    }
}
