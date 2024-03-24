using Game;
using Game.Abstractions;
using UnityEngine;
using Zenject;

namespace MonoInstallers
{
	public class GameMonoInstaller : MonoInstaller
	{
		[SerializeField] private PlanetSpawner _planetSpawner;
		
		public override void InstallBindings()
		{
			Container.BindInterfacesTo<IPlanetSpawner>().FromInstance(_planetSpawner).AsSingle();
		}
	}
}