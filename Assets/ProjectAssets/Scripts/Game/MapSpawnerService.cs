using System.Collections.Generic;
using System.Linq;
using Fusion;
using Game.Abstractions;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game
{
	public class MapSpawnerService : NetworkBehaviour, IMapSpawnerService
	{
		[SerializeField] private NetworkPrefabRef _planet;
		
		private ReactiveProperty<IReadOnlyList<Planet>> _planets = new();

		[Inject] private DiContainer _diContainer;

		public IReactiveProperty<IReadOnlyList<Planet>> Planets => _planets;
		
		public override void Spawned()
		{
			base.Spawned();
			
			SpawnPlanets(0);
		}

		public void SpawnPlanets(int playerCount)
		{
			if (Runner.IsServer)
			{
				SpawnPlanet();
			}
		}
		
		private async void SpawnPlanet()
		{
			await Runner.SpawnAsync(_planet, null, null, PlayerRef.None,
				onBeforeSpawned: (_, map) =>
				{
					_planets.Value = map.gameObject.GetComponentsInChildren<Planet>().ToList();
					foreach (var planet in _planets.Value)
					{
						_diContainer.Inject(planet);
					}
				});
		}
	}
}