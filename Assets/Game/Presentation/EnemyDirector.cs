using System;
using Assets.Game.Domain;
using UniRx;
using UnityEngine;
using Zenject;

namespace Assets.Game.Presentation
{
	public class EnemyDirector : MonoBehaviour
	{
		[Serializable]
		public class EnemyParameters
		{
			public float EnemyMass = 1.0f;
			public float EnemyRagdollLimbMass = 0.1f;
			public float EnemySpeed = 100f;
			public float EnemyScale = 1.0f;
		}

		public float SpawnDelaySeconds = 2.0f;
		public float MinSpawnDelayOffsetSeconds = 0f;
		public float MaxSpawnDalayOffsetSeconds = 5.0f;

		public EnemyParameters EnemySettings;

		[Inject]
		public IEnemyCommands EnemyCommands { private get; set; }

		[Inject]
		public IEnemyEvents EnemyEvents { private get; set; }

		private void Start()
		{
			EnemyEvents
				.OfType<EnemyEvent, EnemyEvent.SpawningStarted>()
				.Subscribe(_ => StartSpawning())
				.AddTo(this);
		}

		private void StartSpawning()
		{
			var delayTimeSpan = TimeSpan.FromSeconds(SpawnDelaySeconds);

			Observable.Timer(TimeSpan.Zero, delayTimeSpan)
				.Subscribe(_ => Invoke(nameof(SpawnAnEnemy),
					UnityEngine.Random.Range(MinSpawnDelayOffsetSeconds, MaxSpawnDalayOffsetSeconds)))
				.AddTo(this);
		}

		public void SpawnAnEnemy()
			=> EnemyCommands.SpawnEnemy(transform.position, new IEnemyCommands.EnemyParameters(EnemySettings.EnemyMass, EnemySettings.EnemyRagdollLimbMass, EnemySettings.EnemySpeed, EnemySettings.EnemyScale));
	}

}
