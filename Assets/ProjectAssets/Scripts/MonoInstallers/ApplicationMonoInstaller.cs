using Game.Abstractions;
using Zenject;

namespace MonoInstallers
{
	public class ApplicationMonoInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesTo<IGameStateService>().AsSingle();
		}
	}
}