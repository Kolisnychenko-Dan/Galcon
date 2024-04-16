using System;
using System.Linq;
using Fusion;
using Game.Abstractions;
using Networking.Abstractions;
using Tools;
using UniRx;
using UnityEngine;
using Zenject;

namespace Networking
{
	public class NetworkLobbyService : NetworkBehaviour, ILobbyService, IPlayerJoined, IPlayerLeft
	{
		private readonly BijectiveDictionary<PlayerRef, int> _playerRefsToIds = new();
		private readonly Subject<LobbyInfo> _allPlayersJoinedSubject = new();
		private LobbyInfo _lobbyInfo;
		
		[Inject] private IGameStateService _gameStateService;
			
		public IReadOnlyBijectiveDictionary<PlayerRef, int> PlayerRefToIdMap => _playerRefsToIds;
		
		public IObservable<LobbyInfo> OnAllPlayersJoined => _allPlayersJoinedSubject;

		public void Initialize(LobbyInfo lobbyInfo)
		{
			_lobbyInfo = lobbyInfo;
			_playerRefsToIds.Clear();
			_gameStateService.ChangeGameState(Trigger.ConnectToRoom);
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