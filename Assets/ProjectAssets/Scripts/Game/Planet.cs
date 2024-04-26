using System;
using Fusion;
using UnityEngine;

namespace Game
{
	public class Planet : NetworkBehaviour
	{
		[Header("Planet Settings")]
		[SerializeField] private int _startingOwnerId = -1;
		[SerializeField] private float _productionRateMultiplier = 1;
		[SerializeField] private float _startingValue;
		
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
	}
}