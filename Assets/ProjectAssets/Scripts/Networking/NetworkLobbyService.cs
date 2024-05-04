using System;
using System.Collections.Generic;
using System.Linq;
using Abstractions;
using Fusion;
using Game.Abstractions;
using MonoInstallers;
using Networking.Abstractions;
using Tools;
using UniRx;
using UnityEngine;
using Zenject;

namespace Networking
{
	public class NetworkLobbyService : NetworkBehaviour, ILobbyService, IPlayerJoined, IPlayerLeft
	{
		private LobbyInfo _lobbyInfo;
		private CompositeDisposable _playerReadyDisposable = new ();
		private Dictionary<PlayerRef, bool> _playerReadyMap  = new ();
		
		[Inject] private IAppStateService _appStateService;

		[Networked, Capacity(4)] public NetworkDictionary<PlayerRef, int> PlayerRefToIdMap => default;

		public override void Spawned()
		{
			base.Spawned();
			
			DontDestroyOnLoad(gameObject);

			if (Runner.IsClient)
			{
				_appStateService = ApplicationMonoInstaller.DiContainer.Resolve<IAppStateService>();

				if (Runner.IsPlayer)
				{
					PlayerLoadedRpc(Runner.LocalPlayer);
				}
			}
		}

		[Rpc(RpcSources.Proxies, RpcTargets.StateAuthority)]
		public void PlayerLoadedRpc(PlayerRef player)
		{
			_playerReadyMap.Add(player, true);
		}

		public void Initialize(LobbyInfo lobbyInfo)
		{
			if (!Runner.IsServer) return;
				
			_lobbyInfo = lobbyInfo;
			
			PlayerRefToIdMap.Clear();
			_playerReadyMap.Clear();
			
			_appStateService.ChangeGameState(Trigger.ConnectToRoom);

			if (Runner.IsPlayer)
			{
				PlayerRefToIdMap.Add(Runner.LocalPlayer, 0);
				_playerReadyMap.Add(Runner.LocalPlayer, true);
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
				_playerReadyDisposable.Clear();
				Observable.EveryUpdate()
					.Where(_ => _playerReadyMap.Count == _lobbyInfo.PlayerCount &&
						_playerReadyMap.All(kp => kp.Value))
					.Subscribe(_ =>
					{
						OnAllPlayersJoinedRpc();
						_playerReadyDisposable.Clear();
					})
					.AddTo(_playerReadyDisposable);
			}
		}
		
		[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
		public void OnAllPlayersJoinedRpc()
		{
			_appStateService.ChangeGameState(Trigger.StartGame);
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