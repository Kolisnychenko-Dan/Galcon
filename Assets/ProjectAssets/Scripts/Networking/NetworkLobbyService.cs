using System.Linq;
using Abstractions;
using Fusion;
using Networking.Abstractions;
using Tools;
using UnityEngine;
using Zenject;

namespace Networking
{
	public class NetworkLobbyService : NetworkBehaviour, ILobbyService, IPlayerJoined, IPlayerLeft
	{
		private LobbyInfo _lobbyInfo;
		
		[Inject] private IAppStateService _appStateService;

		[Networked, Capacity(4)] public NetworkDictionary<PlayerRef, int> PlayerRefToIdMap => default;

		public override void Spawned()
		{
			base.Spawned();
			
			DontDestroyOnLoad(gameObject);
		}

		public void Initialize(LobbyInfo lobbyInfo)
		{
			if (!Runner.IsServer) return;
				
			_lobbyInfo = lobbyInfo;
			PlayerRefToIdMap.Clear();
			_appStateService.ChangeGameState(Trigger.ConnectToRoom);

			if (Runner.IsPlayer)
			{
				PlayerRefToIdMap.Add(Runner.LocalPlayer, 0);
			} 
		}
		
		public void PlayerJoined(PlayerRef player)
		{
			if (!Runner.IsServer) return;
			
			int newPlayerId = GenerateNewPlayerId();
			PlayerRefToIdMap.Add(player, newPlayerId);

			Debug.Log($"Player {newPlayerId} has joined the lobby.");
			
			if (_lobbyInfo.PlayerCount == Runner.ActivePlayers.Count())
			{
				_appStateService.ChangeGameState(Trigger.StartGame);
			}
		}

		public void PlayerLeft(PlayerRef player)
		{
			if (!Runner.IsServer) return;
			
			PlayerRefToIdMap.Remove(player);
			Debug.Log($"Player has left the lobby.");
		}
		
		private int GenerateNewPlayerId()
		{
			return PlayerRefToIdMap.Max(kvp => kvp.Value) + 1;
		}
	}
}