using Game.Abstractions;
using MonoInstallers;
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
		[SerializeField] private JoinLobbyPopup _joinLobbyPopupPrefab;
		
		private IPopupService _popupService;
		private IPopupService PopupService => _popupService ??= PersistentUIMonoInstaller.DiContainer.Resolve<IPopupService>();
		
		//[Inject] private IGameStateService _gameStateService;

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
			PopupService.CreatePopup(_joinLobbyPopupPrefab);
		}
	}
}