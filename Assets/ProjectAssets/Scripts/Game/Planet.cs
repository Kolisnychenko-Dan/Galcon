using System;
using Fusion;
using UnityEngine;

namespace Game
{
	public class Planet : NetworkBehaviour
	{
		[SerializeField] private int _startingOwnerId = -1;
		[SerializeField] private float _productionRateMultiplier = 1;
		[SerializeField] private float _startingValue;

		[SerializeField] private Hitbox _hitbox;
		[SerializeField] private HitboxRoot _hitboxRoot;
		
		[Networked] public float Value { get; private set; }
		[Networked] public int OwnerId { get; private set; }

		public void SubtractValue(int value)
		{
			if (Runner.IsServer)
			{
				Value = Mathf.Max(0, Value - value);
			}
		}

		public void OnShipReached(int shipOwnerId)
		{
			if (Runner.IsServer)
			{
				if (OwnerId != shipOwnerId)
				{
					if (--Value < 0)
					{
						OwnerId = shipOwnerId;
						Value = 0;
					}
				}
				else
				{
					++Value;
				}
			}
		}
		
		public override void Spawned()
		{
			_hitbox.SphereRadius *= transform.localScale.x;
			_hitboxRoot.BroadRadius *= transform.localScale.x;
			
			if (Runner.IsServer)
			{
				Value = _startingValue;
				OwnerId = _startingOwnerId;
			}
		}

		public override void FixedUpdateNetwork()
		{
			if (Runner.IsServer && OwnerId != Constants.NoOwnerId)
			{
				Value += Runner.DeltaTime * MathF.Pow(transform.localScale.x * 2f, 2) * _productionRateMultiplier;
			}
		}
	}
}