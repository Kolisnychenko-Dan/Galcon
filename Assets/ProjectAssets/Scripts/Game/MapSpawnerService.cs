using System.Collections.Generic;
using System.Linq;
using Fusion;
using Game.Abstractions;
using Networking.Abstractions;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game
{
	public class MapSpawnerService : NetworkBehaviour, IMapSpawnerService
	{
		[SerializeField] private NetworkPrefabRef[] _maps;
		
		private ILobbyService _lobbyService;
		
		private ReactiveProperty<IReadOnlyList<Planet>> _planets = new();

		[Inject] private INetworkRunnerService _networkRunnerService;
		[Inject] private DiContainer _diContainer;

		public IReactiveProperty<IReadOnlyList<Planet>> Planets => _planets;
		
		public override void Spawned()
		{
			base.Spawned();
			
			_lobbyService ??= _networkRunnerService.LobbyService;
			
			SpawnMap(_lobbyService.PlayerRefToIdMap.Count);
		}

		public void SpawnMap(int playerCount)
		{
			if (Runner.IsServer)
			{
				SpawnMapAsync(playerCount);
			}
		}
		
		private async void SpawnMapAsync(int playerCount)
		{
			await Runner.SpawnAsync(_maps[playerCount - 2], null, null, PlayerRef.None,
				onBeforeSpawned: (_, map) =>
				{
					var planets = map.gameObject.GetComponentsInChildren<Planet>().ToList();
					_planets.Value = planets;
					foreach (var planet in _planets.Value)
					{
						_diContainer.Inject(planet);
					}
				});
		}
	}
}