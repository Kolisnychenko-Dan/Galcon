using Fusion;
using Networking.Abstractions;
using UI.Abstractions;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
	public class JoinLobbyPopup : Popup
	{
		[SerializeField] private InputField _lobbyId;
		[SerializeField] private Button _joinButton;
		[SerializeField] private Button _createButton;
		[SerializeField] private CreateLobbyPopup _createLobbyPopupPrefab;

		[Inject] private IPopupService _popupService;
		[Inject] private INetworkRunnerService _networkRunnerService;

		protected override void Start()
		{
			base.Start();

			_joinButton.OnClickAsObservable()
				.Subscribe(_ => OnJoinLobbyClicked())
				.AddTo(gameObject);
			
			_createButton.OnClickAsObservable()
				.Subscribe(_ => OnCreateLobbyClicked())
				.AddTo(gameObject);
		}

		private async void OnJoinLobbyClicked()
		{
			if (!string.IsNullOrWhiteSpace(_lobbyId.text))
			{
				var gameArgs = new StartGameArgs
					{
						GameMode = GameMode.Client,
						SessionName = _lobbyId.text
						//ObjectProvider = _runnerInstance.GetComponent<NetworkObjectPoolDefault>(),
					};

				await _networkRunnerService.StartGame(gameArgs);
				
				Close();
			}
		}
		
		private void OnCreateLobbyClicked()
		{
			Close();

			_popupService.CreatePopup(_createLobbyPopupPrefab);
		}
	}
}