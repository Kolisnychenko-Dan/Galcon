using Game;
using Networking;
using Tools;
using UnityEngine;
using Zenject;

namespace MonoInstallers
{
	public class ApplicationMonoInstaller : MonoInstaller
	{
		[SerializeField] private NetworkRunnerService _runnerService;
		
		public static DiContainer DiContainer;
		
		public override void InstallBindings()
		{
			DiContainer = Container;
			
			Container.BindInterfacesTo<AppStateService>().AsSingle();
			Container.BindInterfacesTo<EventManager>().AsSingle();
			Container.BindInterfacesAndSelfTo<NetworkRunnerService>().FromInstance(_runnerService).AsSingle();
		}
	}
}