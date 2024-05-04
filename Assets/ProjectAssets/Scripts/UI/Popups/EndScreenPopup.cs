using System;
using TMPro;
using Tools;
using UI.Abstractions;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class EndScreenPopup : Popup
	{
		[SerializeField] private Button _okButton;
		[SerializeField] private Button _rerunButton;
		[SerializeField] private TextMeshProUGUI _playerNameText;

		public IObservable<Unit> OnOkClicked => _okButton.OnClickAsObservable();
		public IObservable<Unit> OnRerunClicked => _rerunButton.OnClickAsObservable();
		
		public void Initialize(int winnerId)
		{
			_playerNameText.text = Constants.PlayerIdToNameMap[winnerId];
			_playerNameText.color = Constants.PlayerIdToColorMap[winnerId];
		}
		
		protected override void Start()
		{
			_okButton.OnClickAsObservable()
				.Subscribe(_ =>
				{
					Close();
				})
				.AddTo(gameObject);
			
			_rerunButton.OnClickAsObservable()
				.Subscribe(_ =>
				{
					Close();
				})
				.AddTo(gameObject);
		}
	}
}