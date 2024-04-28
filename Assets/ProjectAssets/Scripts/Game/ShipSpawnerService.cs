using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Fusion;
using Game.Abstractions;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game
{
	public class ShipSpawnerService : NetworkBehaviour, IShipSpawnerService
	{
		[SerializeField] private OwnerIdToShipPrefab[] _shipPrefabs;
		
		[Inject] private IMapSpawnerService _mapSpawnerService;

		private Dictionary<Planet, ShipSpawnPositionGenerator> _planetToSpawnerPositionMap = new ();
		
		public override void Spawned()
		{
			if (Runner.IsServer)
			{
				_mapSpawnerService.Planets.Subscribe(planets =>
				{
					_planetToSpawnerPositionMap.Clear();
					foreach (var planet in planets)
					{
						_planetToSpawnerPositionMap.Add(planet, planet.GetComponent<ShipSpawnPositionGenerator>());
						_planetToSpawnerPositionMap[planet].Initialize(_shipPrefabs[0].ShipPrefab);
					}
				}).AddTo(gameObject);	
			}
		}

		public async UniTask<List<Ship>> SpawnShips(int ownerId, int amount, Planet parentPlanet, Planet toPlanet)
		{
			var positions = _planetToSpawnerPositionMap[parentPlanet].CalculateShipSpawnPositions(toPlanet.gameObject, amount);

			var spawnOps = new NetworkSpawnOp[amount];
			int i = 0;
			foreach (var position in positions)
			{
				spawnOps[i] = Runner.SpawnAsync(_shipPrefabs.First(osp => osp.OwnerId == ownerId).ShipPrefab,
					position);
				++i;
			}

			await UniTask.WaitUntil(() => spawnOps.All(op => op.Status == NetworkSpawnStatus.Spawned));
			return spawnOps.Select(spO => spO.Object.GetComponent<Ship>()).ToList();
		}
		
		[Serializable]
		public class OwnerIdToShipPrefab
		{
			public int OwnerId;
			public GameObject ShipPrefab;
		}
	}
}