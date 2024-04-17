using Game;
using UnityEngine;
using Zenject;

namespace MonoInstallers
{
	public class GameMonoInstaller : MonoInstaller
	{
		[SerializeField] private MapSpawner _mapSpawner;
		
		public override void InstallBindings()
		{
			Container.BindInterfacesTo<MapSpawner>().FromInstance(_mapSpawner).AsSingle();
		}
	}
}