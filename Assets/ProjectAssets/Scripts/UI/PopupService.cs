using UI.Abstractions;
using UnityEngine;
using Zenject;

namespace UI
{
	public class PopupService : MonoBehaviour, IPopupService
	{
		[SerializeField] private GameObject _root;
		[Inject] private DiContainer _diContainer;

		public T CreatePopup<T>(T prefab, Transform parent = null) where T : Object
		{
			parent ??= _root.transform;

			switch (prefab)
			{
				case GameObject o:
				{
					return _diContainer.InstantiatePrefab(o, parent) as T;
				}
				case Component component:
				{
					var instance = _diContainer.InstantiatePrefab(component.gameObject, parent);
					var result = instance.GetComponent<T>();
					return result;
				}
				default:
					throw new System.InvalidCastException();
			}
		}
	}
}