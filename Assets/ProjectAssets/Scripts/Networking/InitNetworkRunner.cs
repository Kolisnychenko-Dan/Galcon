using Fusion;
using TMPro;
using UnityEngine;

namespace Networking
{
	public class InitNetworkRunner : MonoBehaviour
	{
		[SerializeField] private NetworkRunner _networkRunnerPrefab;
		[SerializeField] private TMP_InputField _nickName;
		[SerializeField] private TextMeshProUGUI _nickNamePlaceholder;
		[SerializeField] private string _roomName;
		[SerializeField] private string _gameSceneName;
    
		private NetworkRunner _runnerInstance = null;
    
		public void Start()
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

			if (_runnerInstance.IsServer)
			{
				_runnerInstance.LoadScene(_gameSceneName);
			}
        }
	}
}