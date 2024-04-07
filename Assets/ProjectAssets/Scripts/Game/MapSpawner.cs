using System.Collections.Generic;
using Fusion;
using Game.Abstractions;
using UnityEngine;

namespace Game
{
	public class MapSpawner : NetworkBehaviour, IPlanetSpawner
	{
		[SerializeField] private NetworkPrefabRef _planet;
		
		private List<NetworkId> _planets = new();

		public override void Spawned()
		{
			base.Spawned();
			
			SpawnPlanets(0);
		}

		public void SpawnPlanets(int playerCount)
		{
			if (Object.HasStateAuthority == false) return;
			
			SpawnPlanet();
		}
		
		private void SpawnPlanet()
		{
			var planet = Runner.Spawn(_planet, null, null, PlayerRef.None,
				onBeforeSpawned: (_,_) => {});

			_planets.Add(planet.Id);
		}
	}
}