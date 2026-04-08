using System;
using Colony;
using Core;
using Extensions;
using Resources;
using Zenject;

namespace Infrastructure.Installers
{
    public class ColonyBootstrapper : IInitializable, IDisposable
    {
        private readonly IBugSpawnService _spawnService;
        private readonly IColonyService _colonyService;
        private readonly IResourceSpawnService _resourceSpawnService;
        private readonly ColonyConfig _colonyConfig;

        public ColonyBootstrapper(
            IBugSpawnService spawnService,
            IColonyService colonyService,
            IResourceSpawnService resourceSpawnService,
            ColonyConfig colonyConfig)
        {
            _spawnService = spawnService;
            _colonyService = colonyService;
            _resourceSpawnService = resourceSpawnService;
            _colonyConfig = colonyConfig;
        }

        public void Initialize()
        {
            _colonyService.OnColonyExtinct += HandleColonyExtinct;
            _spawnService.Spawn(BugType.Worker, _colonyConfig.SpawnCenter.SampleNavMesh());
            _resourceSpawnService.StartSpawning();
        }

        public void Dispose()
        {
            _colonyService.OnColonyExtinct -= HandleColonyExtinct;
        }

        private void HandleColonyExtinct()
        {
            var position = _colonyConfig.SpawnCenter.GetRandomNavMeshPoint(_colonyConfig.SpawnAreaRadius);
            _spawnService.Spawn(BugType.Worker, position);
        }
    }
}
