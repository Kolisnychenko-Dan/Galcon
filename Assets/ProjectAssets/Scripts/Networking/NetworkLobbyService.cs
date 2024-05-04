using System;
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
		
		[Inject] private IAppStateService _appStateService;

		[Networked, Capacity(4)] public NetworkDictionary<PlayerRef, int> PlayerRefToIdMap => default;
		[Networked, Capacity(4)] public NetworkDictionary<PlayerRef, bool> PlayerReadyMap => default;

		public override void Spawned()
		{
			base.Spawned();
			
			DontDestroyOnLoad(gameObject);

			if (Runner.IsClient)
			{
				_appStateService = ApplicationMonoInstaller.DiContainer.Resolve<IAppStateService>();

				if (Runner.IsPlayer)
				{
					PlayerReadyMap.Add(Runner.LocalPlayer, true);
				}
			}
		}

		public void Initialize(LobbyInfo lobbyInfo)
		{
			if (!Runner.IsServer) return;
				
			_lobbyInfo = lobbyInfo;
			
			PlayerRefToIdMap.Clear();
			PlayerReadyMap.Clear();
			
			_appStateService.ChangeGameState(Trigger.ConnectToRoom);

			if (Runner.IsPlayer)
			{
				PlayerRefToIdMap.Add(Runner.LocalPlayer, 0);
				PlayerReadyMap.Add(Runner.LocalPlayer, true);
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
					.Where(_ => PlayerReadyMap.All(kp => kp.Value))
					.Skip(1)
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