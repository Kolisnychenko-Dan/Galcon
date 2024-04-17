using UI;
using UnityEngine;
using Zenject;

namespace MonoInstallers
{
	public class PersistentUIMonoInstaller : MonoInstaller
	{
		[SerializeField] private PopupService _popupService;

		public static DiContainer DiContainer;
		
		public override void InstallBindings()
		{
			DiContainer = Container;
			Container.BindInterfacesTo<PopupService>().FromInstance(_popupService).AsSingle();
		}
	}
}