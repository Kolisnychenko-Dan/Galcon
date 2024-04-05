using Game;
using Networking;
using UnityEngine;
using Zenject;

namespace MonoInstallers
{
	public class ApplicationMonoInstaller : MonoInstaller
	{
		[SerializeField] private NetworkRunnerService _runnerService;
		
		public override void InstallBindings()
		{
			Container.BindInterfacesTo<GameStateService>().AsSingle();
			Container.BindInterfacesAndSelfTo<NetworkRunnerService>().FromInstance(_runnerService).AsSingle();
		}
	}
}