using System.Linq;
using Fusion;
using Game.Abstractions;
using Tools;
using Zenject;
using UniRx;

namespace Game
{
	public class GameStateService : NetworkBehaviour, IGameStateService
	{
		[Inject] private IMapSpawnerService _mapSpawnerService;

		public override void Spawned()
		{
			base.Spawned();

			EventManager.Instance.GetEvent<Planet>(EventNames.PlanetCaptured)
				.Subscribe(OnPlanetCaptured)
				.AddTo(gameObject);
		}

		private void OnPlanetCaptured(Planet planet)
		{
			bool isWin = CheckWinConditionForPlayer(planet.OwnerId);
			if (isWin)
			{
				
			}
		}

		private bool CheckWinConditionForPlayer(int ownerId)
		{
			return _mapSpawnerService.Planets.Value.All(planet =>
				planet.OwnerId == ownerId || planet.OwnerId == Constants.NoOwnerId);
		}
	}
}