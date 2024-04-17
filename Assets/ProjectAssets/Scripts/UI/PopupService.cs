using UI.Abstractions;
using UniRx;
using UnityEngine;
using Zenject;
using UniRx.Triggers;

namespace UI
{
	public class PopupService : MonoBehaviour, IPopupService
	{
		[SerializeField] private GameObject _root;
		[SerializeField] private GameObject _tint;
		
		[Inject] private DiContainer _diContainer;

		private ReactiveProperty<bool> _tintActive = new (false);
		private int _popupCount;

		private void Start()
		{
			_tintActive.Subscribe(active => _tint.SetActive(active)).AddTo(gameObject);
		}
		
		public T CreatePopup<T>(T prefab, Transform parent = null) where T : Object
		{
			parent ??= _root.transform;

			switch (prefab)
			{
				case Popup popup:
				{
					var instance = _diContainer.InstantiatePrefab(popup.gameObject, parent);
					var result = instance.GetComponent<T>();
					
					(result as Component).OnDestroyAsObservable().Subscribe(_ => PopupClosed()).AddTo(gameObject);
					
					PopupOpened();

					return result;
				}
				case GameObject o:
				{
					return _diContainer.InstantiatePrefab(o, parent) as T;
				}
				default:
					throw new System.InvalidCastException();
			}
		}
		
		private void PopupOpened()
		{
			_popupCount++;
			_tintActive.Value = true;
		}

		private void PopupClosed()
		{
			_popupCount--;
			Observable.NextFrame().Subscribe(_ =>
			{
				if (_popupCount == 0)
				{
					_tintActive.Value = false;
				}
			}).AddTo(gameObject);
		}
	}
}