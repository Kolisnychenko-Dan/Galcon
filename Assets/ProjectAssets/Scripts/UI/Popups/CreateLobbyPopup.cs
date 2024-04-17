using Fusion;
using Networking.Abstractions;
using UI.Abstractions;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
	public class CreateLobbyPopup : Popup
	{
		[SerializeField] private InputField _lobbyId;
		[SerializeField] private Button _createButton;
		[SerializeField] private JoinLobbyPopup _joinLobbyPopupPrefab;

		[Inject] private DiContainer _container;
		[Inject] private INetworkRunnerService _networkRunnerService;

		protected override void Start()
		{
			base.Start();
			
			_createButton.OnClickAsObservable()
				.Subscribe(_ => OnCreateLobbyClicked())
				.AddTo(gameObject);
			
			string gennedId = System.Guid.NewGuid().ToString()[..6];
			_lobbyId.text = gennedId;
		}

		protected override void Close()
		{
			base.Close();

			var popupService = _container.Resolve<IPopupService>(); 
			popupService.CreatePopup(_joinLobbyPopupPrefab);
		}

		private async void OnCreateLobbyClicked()
		{
			if (!string.IsNullOrWhiteSpace(_lobbyId.text))
			{
				var gameArgs = new StartGameArgs
				{
					GameMode = GameMode.Host,
					SessionName = _lobbyId.text,
					PlayerCount = 2
					//ObjectProvider = _runnerInstance.GetComponent<NetworkObjectPoolDefault>(),
				};

				await _networkRunnerService.StartGame(gameArgs);
				
				base.Close();
			}
		}
	}
}