using System.Collections.Generic;
using System.Linq;
using Fusion;
using Game.Abstractions;
using UnityEngine;
using Zenject;

namespace Game
{
	public class MapSpawner : NetworkBehaviour, IMapSpawner
	{
		[SerializeField] private NetworkPrefabRef _planet;
		
		private List<Planet> _planets = new();

		[Inject] private DiContainer _diContainer;
		
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
		
		private void SpawnPlanet()
		{
			var map = Runner.Spawn(_planet, null, null, PlayerRef.None,
				onBeforeSpawned: (_,_) => {});

			_planets = map.gameObject.GetComponentsInChildren<Planet>().ToList();
			foreach (var planet in _planets)
			{
				_diContainer.Inject(planet);
			}
		}
	}
}