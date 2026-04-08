using Bugs.Core;
using Bugs.Interaction;
using Bugs.Predator;
using Bugs.Worker;
using Colony;
using Gameplay.Services.GameTime;
using Resources;
using Stats;
using UnityEngine;
using Zenject;

namespace Infrastructure.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private Bug _workerBugPrefab;
        [SerializeField] private Bug _predatorBugPrefab;
        [SerializeField] private FoodResource _foodPrefab;
        [SerializeField] private WorkerConfig _workerConfig;
        [SerializeField] private PredatorConfig _predatorConfig;
        [SerializeField] private ResourceConfig _resourceConfig;
        [SerializeField] private ColonyConfig _colonyConfig;

        public override void InstallBindings()
        {
            BindConfigs();
            BindServices();
            BindFactories();
            BindBootstrapper();
        }

        private void BindConfigs()
        {
            Container.BindInstance(_workerConfig).AsSingle().Lazy();
            Container.BindInstance(_predatorConfig).AsSingle().Lazy();
            Container.BindInstance(_resourceConfig).AsSingle().Lazy();
            Container.BindInstance(_colonyConfig).AsSingle().Lazy();
        }

        private void BindServices()
        {
            Container.Bind<ITicker>().To<MonoTicker>().FromNewComponentOnNewGameObject().WithGameObjectName("Ticker").AsSingle();
            Container.Bind<IColonyStatsService>().To<ColonyStatsService>().AsSingle();
            Container.Bind<IResourceRegistry>().To<ResourceRegistry>().AsSingle();
            Container.Bind<IBugSpawnService>().To<BugSpawnService>().AsSingle();
            Container.Bind<IColonyService>().To<ColonyService>().AsSingle();
            Container.Bind<IMutationService>().To<MutationService>().AsSingle();
            Container.Bind<IInteractionService>().To<InteractionService>().AsSingle();
            Container.BindInterfacesAndSelfTo<ResourceSpawnService>().AsSingle();
        }

        private void BindFactories()
        {
            Container.BindFactory<Bug, Bug.WorkerFactory>().FromComponentInNewPrefab(_workerBugPrefab);
            Container.BindFactory<Bug, Bug.PredatorFactory>().FromComponentInNewPrefab(_predatorBugPrefab);
            Container.BindFactory<FoodResource, FoodResource.Factory>().FromComponentInNewPrefab(_foodPrefab);
        }

        private void BindBootstrapper()
        {
            Container.BindInterfacesTo<ColonyBootstrapper>().AsSingle().NonLazy();
        }
    }
}
