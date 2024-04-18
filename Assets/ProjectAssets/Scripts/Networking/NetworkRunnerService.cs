using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Fusion;
using Game.Abstractions;
using Networking.Abstractions;
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

		[Inject] private IGameStateService _gameStateService;
		[Inject] private DiContainer _diContainer;

		public NetworkRunner GetCurrentNetworkRunner()
		{
			return _runnerInstance;
		}

		public NetworkLobbyService LobbyService => _networkLobbyService;

		public UniTask LoadNetworkScene(string sceneName, LoadSceneMode loadSceneMode)
		{
			if (_runnerInstance.IsServer)
			{
				return _runnerInstance.LoadScene(sceneName, loadSceneMode).ToUniTask();
			}

			return UniTask.CompletedTask;
		}

		// public void GameStart()
		// {
		// 	var gameArgs = new StartGameArgs()
		// 	{
		// 		GameMode = GameMode.AutoHostOrClient,
		// 		SessionName = "Booba",
		// 		//ObjectProvider = _runnerInstance.GetComponent<NetworkObjectPoolDefault>(),
		// 	};
		// 	StartGame(gameArgs);
		// }

		public async Task<StartGameResult> StartGame(StartGameArgs gameArgs)
		{
			_runnerInstance = FindObjectOfType<NetworkRunner>();
			if (_runnerInstance == null)
			{
				_runnerInstance = Instantiate(_networkRunnerPrefab);
			}

			_runnerInstance.ProvideInput = true;

			var result = await _runnerInstance.StartGame(gameArgs);

			if (result.Ok && _runnerInstance.IsServer)
			{
				var lobbyService = _runnerInstance.Spawn(_networkLobbyServicePrefab, null, null, PlayerRef.None,
					onBeforeSpawned: (_,_) => {});

				_networkLobbyService = lobbyService.GetComponent<NetworkLobbyService>();
				_diContainer.Inject(_networkLobbyService);
				_networkLobbyService.Initialize(new LobbyInfo {PlayerCount = gameArgs.PlayerCount.Value});
			}

			return result;
		}
	}
}