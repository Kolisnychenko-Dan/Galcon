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
		private readonly BijectiveDictionary<PlayerRef, int> _playerRefsToIds = new();
		private LobbyInfo _lobbyInfo;
		
		[Inject] private IGameStateService _gameStateService;
		
		public IReadOnlyBijectiveDictionary<PlayerRef, int> PlayerRefToIdMap => _playerRefsToIds;

		public void Initialize(LobbyInfo lobbyInfo)
		{
			_lobbyInfo = lobbyInfo;
			_playerRefsToIds.Clear();
			_gameStateService.ChangeGameState(Trigger.ConnectToRoom);

			if (Runner.IsServer && Runner.IsPlayer)
			{
				_playerRefsToIds.Add(Runner.LocalPlayer, 0);
			} 
		}
		
		public void PlayerJoined(PlayerRef player)
		{
			int newPlayerId = GenerateNewPlayerId();
			_playerRefsToIds.Add(player, newPlayerId);

			Debug.Log($"Player {newPlayerId} has joined the lobby.");
			
			if (_lobbyInfo.PlayerCount == Runner.ActivePlayers.Count())
			{
				_gameStateService.ChangeGameState(Trigger.StartGame);
			}
		}

		public void PlayerLeft(PlayerRef player)
		{
			_playerRefsToIds.RemoveByKey(player);
			Debug.Log($"Player has left the lobby.");
		}
		
		private int GenerateNewPlayerId()
		{
			return _playerRefsToIds.Max(kvp => kvp.Value) + 1;
		}
	}
}