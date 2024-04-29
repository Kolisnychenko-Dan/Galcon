using System;
using System.Threading.Tasks;
using Abstractions;
using Cysharp.Threading.Tasks;
using Fusion;
using Networking.Abstractions;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Networking
{
	public class NetworkRunnerService : MonoBehaviour, INetworkRunnerService
	{
		[SerializeField] private NetworkRunner _networkRunnerPrefab;
		[SerializeField] private NetworkLobbyService _networkLobbyServicePrefab;
    
		private NetworkRunner _runnerInstance = null;
		private NetworkLobbyService _networkLobbyService;

		[Inject] private IAppStateService _appStateService;
		[Inject] private DiContainer _diContainer;

		public NetworkRunner GetCurrentNetworkRunner()
		{
			return _runnerInstance;
		}

		public NetworkLobbyService LobbyService => _networkLobbyService ??= FindFirstObjectByType<NetworkLobbyService>();

		public UniTask LoadNetworkScene(string sceneName, LoadSceneMode loadSceneMode)
		{
			if (_runnerInstance.IsServer)
			{
				return _runnerInstance.LoadScene(sceneName, loadSceneMode).ToUniTask();
			}

			return UniTask.CompletedTask;
		}

		public async Task<StartGameResult> StartGame(StartGameArgs gameArgs)
		{
			_runnerInstance = FindFirstObjectByType<NetworkRunner>();
			if (_runnerInstance == null)
			{
				_runnerInstance = Instantiate(_networkRunnerPrefab);
			}

			_runnerInstance.ProvideInput = true;

			var result = await _runnerInstance.StartGame(gameArgs);

			if (result.Ok && _runnerInstance.IsServer)
			{
				if(_networkLobbyService == null)
				{
					await _runnerInstance.SpawnAsync(_networkLobbyServicePrefab, null, null, PlayerRef.None,
						onBeforeSpawned: (_, ls) =>
						{
							_networkLobbyService = ls.GetComponent<NetworkLobbyService>();
							_diContainer.Inject(_networkLobbyService);
							_networkLobbyService.Initialize(new LobbyInfo {PlayerCount = gameArgs.PlayerCount.Value});
						});
				}
				else
				{
					_networkLobbyService.Initialize(new LobbyInfo {PlayerCount = gameArgs.PlayerCount.Value});
				}
			}

			return result;
		}
	}
}