using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Abstractions
{
	public class Popup : MonoBehaviour
	{
		[SerializeField] protected Button _closeButton;

		protected virtual void Start()
		{
			_closeButton.OnClickAsObservable()
				.Subscribe(_ => Close())
				.AddTo(gameObject);
		}

		protected virtual void Close()
		{
			Destroy(gameObject);
		}
	}
}