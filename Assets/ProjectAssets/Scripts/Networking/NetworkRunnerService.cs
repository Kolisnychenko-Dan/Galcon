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
		[SerializeField] private string _roomName;
    
		private NetworkRunner _runnerInstance = null;

		[Inject] private IGameStateService _gameStateService;
    
		public NetworkRunner GetCurrentNetworkRunner()
		{
			return _runnerInstance;
		}

		public UniTask LoadNetworkScene(string sceneName, LoadSceneMode loadSceneMode)
		{
			if (_runnerInstance.IsServer)
			{
				return _runnerInstance.LoadScene(sceneName, loadSceneMode).ToUniTask();
			}

			return UniTask.CompletedTask;
		}

		public void GameStart()
		{
			var gameArgs = new StartGameArgs()
			{
				GameMode = GameMode.AutoHostOrClient,
				SessionName = _roomName,
				//ObjectProvider = _runnerInstance.GetComponent<NetworkObjectPoolDefault>(),
			};
			StartGame(gameArgs);
		}

		private async void StartGame(StartGameArgs gameArgs)
		{
			_runnerInstance = FindObjectOfType<NetworkRunner>();
			if (_runnerInstance == null)
			{
				_runnerInstance = Instantiate(_networkRunnerPrefab);
			}

			_runnerInstance.ProvideInput = true;

			await _runnerInstance.StartGame(gameArgs);

			_gameStateService.ChangeGameState(Trigger.StartGame);
        }
	}
}