using Game;
using UnityEngine;
using Zenject;

namespace MonoInstallers
{
	public class GameMonoInstaller : MonoInstaller
	{
		[SerializeField] private MapSpawner _mapSpawner;
		[SerializeField] private PlanetSwipeDetector _swipeDetector;
		
		public override void InstallBindings()
		{
			Container.BindInterfacesTo<MapSpawner>().FromInstance(_mapSpawner).AsSingle();
			Container.BindInterfacesTo<PlanetSwipeDetector>().FromInstance(_swipeDetector).AsSingle();
		}
	}
}