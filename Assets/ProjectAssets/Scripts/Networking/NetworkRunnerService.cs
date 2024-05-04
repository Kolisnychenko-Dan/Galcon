using System;
using System.Threading.Tasks;
using Abstractions;
using Cysharp.Threading.Tasks;
using Fusion;
using MonoInstallers;
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

		public NetworkRunner GetCurrentNetworkRunner()
		{
			return _runnerInstance;
		}

		public NetworkLobbyService LobbyService => _networkLobbyService ??= FindFirstObjectByType<NetworkLobbyService>();

		public UniTask LoadNetworkScene(string sceneName, LoadSceneMode loadSceneMode)
		{
			UniTask waitLoadSceneTask;
			if (_runnerInstance.IsServer)
			{
				waitLoadSceneTask = _runnerInstance.LoadScene(sceneName, loadSceneMode).ToUniTask();
			}
			else
			{
				waitLoadSceneTask = UniTask.WaitUntil(() => SceneManager.GetSceneByName(sceneName).isLoaded);
			}

			return waitLoadSceneTask;
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
							ApplicationMonoInstaller.DiContainer.Inject(_networkLobbyService);
							_networkLobbyService.Initialize(new LobbyInfo {PlayerCount = gameArgs.PlayerCount.Value});
						});
				}
				else
				{
					_networkLobbyService.Initialize(new LobbyInfo {PlayerCount = gameArgs.PlayerCount.Value});
				}
			}
			else if(result.Ok && _runnerInstance.IsClient)
			{
				_appStateService.ChangeGameState(Trigger.ConnectToRoom);
			}

			return result;
		}
	}
}