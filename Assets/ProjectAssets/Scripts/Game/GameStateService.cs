using System.Linq;
using Abstractions;
using Fusion;
using Game.Abstractions;
using MonoInstallers;
using Tools;
using UI;
using UI.Abstractions;
using Zenject;
using UniRx;
using UnityEngine;

namespace Game
{
	public class GameStateService : NetworkBehaviour, IGameStateService
	{
		[SerializeField] private EndScreenPopup _endScreenPopup;

		[Inject] private IAppStateService _appStateService;
		[Inject] private IMapSpawnerService _mapSpawnerService;
		private IPopupService _popupService;

		public override void Spawned()
		{
			base.Spawned();

			_popupService = PersistentUIMonoInstaller.DiContainer.Resolve<IPopupService>();
			
			EventManager.Instance.GetEvent<Planet>(EventNames.PlanetCaptured)
				.Subscribe(OnPlanetCaptured)
				.AddTo(gameObject);
		}

		private void OnPlanetCaptured(Planet planet)
		{
			bool isWin = CheckWinConditionForPlayer(planet.OwnerId);
			if (isWin)
			{
				var endScreenPopup = _popupService.CreatePopup(_endScreenPopup);
				endScreenPopup.Initialize(planet.OwnerId);

				endScreenPopup.OnOkClicked
					.Subscribe(_ => GoToMainMenu())
					.AddTo(endScreenPopup.gameObject);
				  
				endScreenPopup.OnRerunClicked
					.Subscribe(_ => RerunTheMatch())
					.AddTo(endScreenPopup.gameObject);
			}
		}

		private void GoToMainMenu()
		{
			_appStateService.ChangeGameState(Trigger.GoToStartScreen);
		}
		
		private void RerunTheMatch()
		{
			_appStateService.ChangeGameState(Trigger.GoToStartScreen);
		}

		private bool CheckWinConditionForPlayer(int ownerId)
		{
			return _mapSpawnerService.Planets.Value.All(planet =>
				planet.OwnerId == ownerId || planet.OwnerId == Constants.NoOwnerId);
		}
	}
}