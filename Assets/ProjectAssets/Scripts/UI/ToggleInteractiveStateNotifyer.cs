using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	[RequireComponent(typeof(Toggle))]
	public class ToggleInteractiveStateNotifyer : MonoBehaviour
	{
		private bool _prevInteractableState = false;
		private Toggle _currentToggle;

		/// <summary>
		/// Invoked when the interactable state of the <see cref="Toggle"/> is changed
		/// </summary>
		public event Action<bool> OnInteractableStateChanged;

		/// <summary>
		/// Gets and sets interactable state of the <see cref="Toggle"/> 
		/// </summary>
		public bool InteractableState
		{
			get { return _currentToggle.interactable; }
			set { _currentToggle.interactable = value; }
		}

		/// <summary>
		/// Get Observable oven an Interactable state of the toggle 
		/// </summary>
		/// <returns></returns>
		public IObservable<bool> OnInteractableStateAsObservable()
		{
			return Observable.FromEvent<bool>(h => OnInteractableStateChanged += h,
				h => OnInteractableStateChanged -= h);
		}

		private void Awake()
		{
			if (null == _currentToggle)
			{
				_currentToggle = GetComponent<Toggle>();
			}
		}

		protected void Start()
		{
			_prevInteractableState = InteractableState;

			Observable.EveryUpdate().Subscribe(_ =>
			{
				if (_prevInteractableState != InteractableState)
				{
					_prevInteractableState = InteractableState;
					OnInteractableStateChanged?.Invoke(_prevInteractableState);
				}
			}).AddTo(gameObject);
		}
	}
}