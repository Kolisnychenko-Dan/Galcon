using System;
using Fusion;
using TMPro;
using UniRx;
using UnityEngine;

namespace Game
{
	public class Planet : NetworkBehaviour
	{
		[Header("Planet Settings")]
		[SerializeField] private int _startingOwnerId = -1;
		[SerializeField] private float _productionRateMultiplier = 1;
		[SerializeField] private float _startingValue;
		
		[Header("Serialized Components")]
		[SerializeField] private SpriteRenderer _tintSpriteRenderer;
		[SerializeField] private TextMeshProUGUI _valueText;
		
		[Networked] public float Value { get; private set; }
		[Networked] public int OwnerId { get; private set; }
		
		
		public override void Spawned()
		{
			if (Runner.IsServer)
			{
				Value = _startingValue;
				OwnerId = _startingOwnerId;
			}
		}

		public override void FixedUpdateNetwork()
		{
			if (Runner.IsServer)
			{
				Value += Runner.DeltaTime * MathF.Pow(transform.localScale.x * 2f, 2) * _productionRateMultiplier;
			}
		}
		
		private void OnTriggerEnter(Collider other)
		{
			if (Runner.IsServer)
			{
				var ship = other.gameObject.GetComponent<Ship>();
				if (ship != null)
				{
					
				}
			}
		}

		private void Start()
		{
			UpdateView();

			Observable.IntervalFrame(20, FrameCountType.EndOfFrame).Subscribe(_ =>
				{
					UpdateView();
				})
				.AddTo(gameObject);
		}

		private void UpdateView()
		{
			_tintSpriteRenderer.color = Constants.PlayerIdToColorMap[OwnerId];
			_valueText.text = Value.ToString("0");
		}
	}
}