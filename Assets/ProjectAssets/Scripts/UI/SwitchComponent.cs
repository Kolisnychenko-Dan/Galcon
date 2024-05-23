using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class SwitchComponent : MonoBehaviour
	{
		public event Action OnIndexChaged;
		public event Action<bool> OnInteractableChanged;

		[SerializeField] private ToggleInteractiveStateNotifyer[] _items;

		public int Index
		{
			get { return _currentIndex; }
			set
			{
				int result = ClampIndex(value);
				if (_currentIndex != result)
				{
					_currentIndex = result;
					SetIndexWithoutNotify(value);
					OnIndexChaged?.Invoke();
				}
			}
		}

		public void SetIndexWithoutNotify(int index)
		{
			_currentIndex = ClampIndex(index);

			for (int i = 0; i < _items.Length; i++)
			{
				var item = _items[i];
				var itemToggle = item.GetComponent<Toggle>();
				if (itemToggle != null)
				{
					itemToggle.SetIsOnWithoutNotify(i == _currentIndex);
				}
			}
		}

		public void SetIndex(int index)
		{
			_currentIndex = ClampIndex(index);

			for (int i = 0; i < _items.Length; i++)
			{
				var item = _items[i];
				var itemToggle = item.GetComponent<Toggle>();
				if (itemToggle != null)
				{
					itemToggle.isOn = (i == _currentIndex);
				}
			}
		}

		private int _currentIndex = 0;

		public bool Interactable
		{
			get { return _interactable; }
			set
			{
				if (_interactable != value)
				{
					OnInteractableChanged?.Invoke(_interactable);
				}

				_interactable = value;
			}
		}

		private bool _interactable;

		public IObservable<Unit> OnIndexChangedAsObservable()
		{
			return Observable.FromEvent(h => OnIndexChaged += h, h => OnIndexChaged -= h);
		}

		public IObservable<bool> OnInteractableChangedAsObservable()
		{
			return Observable.FromEvent<bool>(h => OnInteractableChanged += h, h => OnInteractableChanged -= h);
		}

		protected void Start()
		{
			foreach (var item in _items)
			{
				item.OnInteractableStateAsObservable().Subscribe(_ =>
					OnToggleInteractiveStateChanged()
				).AddTo(gameObject);

				var itemToggle = item.GetComponent<Toggle>();
				if (null != itemToggle)
				{
					itemToggle.OnValueChangedAsObservable().Subscribe(_ =>
						OnToggleValueChanged()
					).AddTo(gameObject);
				}
			}

			OnIndexChangedAsObservable().Subscribe(_ =>
				{
					if (true == Interactable)
					{
						var currentIndexToggleComponent = _items[_currentIndex].GetComponent<Toggle>();
						if (null != currentIndexToggleComponent)
						{
							currentIndexToggleComponent.isOn = true;
						}
					}
				}
			).AddTo(gameObject);

			OnToggleInteractiveStateChanged();
		}
		
		private int ClampIndex(int index)
		{
			int result = index;
			result = Math.Min(result, _items.Length - 1);
			result = Math.Max(result, 0);
			return result;
		}

		private void OnToggleInteractiveStateChanged()
		{
			bool result = true;
			foreach (var item in _items)
			{
				result = result && item.InteractableState;
				if (false == result)
				{
					break;
				}
			}

			Interactable = result;
		}

		private void OnToggleValueChanged()
		{
			for (int i = 0; i < _items.Length; ++i)
			{
				var toggleItem = _items[i].GetComponent<Toggle>();
				if (true == toggleItem.isOn)
				{
					Index = i;
					break;
				}
			}
		}
	}
}