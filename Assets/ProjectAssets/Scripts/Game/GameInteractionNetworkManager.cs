using System;
using System.Linq;
using Fusion;
using Game.Abstractions;
using ModestTree;
using Zenject;
using UniRx;

namespace Game
{
	public class GameInteractionNetworkManager : NetworkBehaviour
	{
		[Inject] private IPlanetSwipeDetector _planetSwipeDetector;
		[Inject] private IShipSpawnerService _shipSpawnerService;
		[Inject] private IMapSpawnerService _mapSpawnerService;

		public override void Spawned()
		{
			base.Spawned();
			
			_planetSwipeDetector.OnPlanetSwipe.Subscribe(planets =>
				Attack(planets.startPlanet, planets.endPlanet));
		}

		private void Attack(SimulationBehaviour startPlanet, SimulationBehaviour endPlanet)
		{
			AttackRpc(startPlanet.Object.Id, endPlanet.Object.Id);
		}

		private Planet GetServerPlanetById(NetworkId id)
		{
			return _mapSpawnerService.Planets.Value.First(planet => planet.Object.Id == id);
		}

		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public async void AttackRpc(NetworkId startPlanetId, NetworkId endPlanetId)
		{
			var startPlanet = GetServerPlanetById(startPlanetId);
			var endPlanet = GetServerPlanetById(endPlanetId);
			
			int maxShipToSpawn = startPlanet.GetComponent<ShipSpawnPositionGenerator>().MaxShipsSpawn;
			int shipsToSpawn = Math.Min((int)startPlanet.Value / 2, maxShipToSpawn);

			var spawnedShips = await _shipSpawnerService.SpawnShips(startPlanet.OwnerId, shipsToSpawn, startPlanet, endPlanet);
			
			if (spawnedShips != null)
			{
				startPlanet.SubtractValue(shipsToSpawn);
				foreach (var ship in spawnedShips)
				{
					ship.Initialize(endPlanet, startPlanet.OwnerId);
				}
			}
		}
	}
}