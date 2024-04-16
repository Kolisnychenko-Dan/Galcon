using Game.Abstractions;
using Networking;
using UI.Abstractions;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
	public class StartSceneUI : MonoBehaviour
	{
		[SerializeField] private Button _playButton;
		[SerializeField] private Button _accountButton;
		[SerializeField] private Button _settingsButton;
		[SerializeField] private Button _aboutButton;
		
		//[Inject] private IPopupService _popupService;
		[Inject] private IGameStateService _gameStateService;
		[Inject] private NetworkRunnerService _runnerService;

		private void Start()
		{
			_playButton.OnClickAsObservable()
				.Subscribe(_ => OnPlayButtonClicked())
				.AddTo(gameObject);

			_accountButton.OnClickAsObservable()
				.Subscribe(_ => {})
				.AddTo(gameObject);

			_settingsButton.OnClickAsObservable()
				.Subscribe(_ => {})
				.AddTo(gameObject);
			
			_aboutButton.OnClickAsObservable()
				.Subscribe(_ => {})
				.AddTo(gameObject);
		}

		private void OnPlayButtonClicked()
		{	
			_gameStateService.ChangeGameState(Trigger.GoToLobby);
			_runnerService.GameStart();
			_runnerService.LobbyService.Initialize(new LobbyInfo {PlayerCount = 2});
		}
	}
}