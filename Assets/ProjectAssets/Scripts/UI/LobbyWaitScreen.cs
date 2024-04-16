using ProjectAssets.Scripts;
using Tools;
using UniRx;
using UnityEngine;

namespace UI
{
	public class LobbyWaitScreen : MonoBehaviour
	{
		[SerializeField] private GameObject _screen;
		
		private void Start()
		{
			_screen.SetActive(false);
			
			EventManager.Instance.GetEvent<bool>(EventNames.ToggleLobbyWaitScreen)
				.Subscribe(OnToggled)
				.AddTo(gameObject);
		}

		private void OnToggled(bool isOn)
		{
			_screen.SetActive(isOn);
		}
	}
}