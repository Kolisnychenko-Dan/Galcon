using Game;
using UnityEngine;
using Zenject;

namespace MonoInstallers
{
	public class GameMonoInstaller : MonoInstaller
	{
		[SerializeField] private MapSpawnerService _mapSpawnerService;
		[SerializeField] private PlanetSwipeDetector _swipeDetector;
		[SerializeField] private ShipSpawnerService _shipSpawnerService;
		
		public override void InstallBindings()
		{
			Container.BindInterfacesTo<MapSpawnerService>().FromInstance(_mapSpawnerService).AsSingle();
			Container.BindInterfacesTo<PlanetSwipeDetector>().FromInstance(_swipeDetector).AsSingle();
			Container.BindInterfacesTo<ShipSpawnerService>().FromInstance(_shipSpawnerService).AsSingle();
		}
	}
}