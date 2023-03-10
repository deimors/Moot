using Assets.Game.Domain;
using Assets.Game.Infrastructure.Integrations;
using UnityEngine;
using Zenject;

namespace Assets.Game.Infrastructure
{
	public class GameInstaller : MonoInstaller
	{
		public Transform EnemiesContainer;
		public GameObject EnemyPrefab;
		public override void InstallBindings()
		{
			Container.BindAggregate<EnemyAggregate>();
			Container.BindPrefabFactory<EnemyParameters, EnemyFactory>(EnemyPrefab, EnemiesContainer);
			Container.BindIntegration<CreateNewEnemyOnEnemySpawned>();

			Container.BindAggregate<RootlingsAggregate>();
		}
	}
}
