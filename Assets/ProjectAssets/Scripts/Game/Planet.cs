using System;
using System.Linq;
using Fusion;
using Networking.Abstractions;
using Tools;
using UnityEngine;
using Zenject;

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
		[Networked] public PlayerRef PlayerRef { get; private set; }
		
		[Inject] private INetworkRunnerService _networkRunnerService;
		
		private ILobbyService _lobbyService;

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
						
						EventManager.Instance.EmitEvent(EventNames.PlanetCaptured, this);
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
				_lobbyService = _networkRunnerService.LobbyService;
				
				Value = _startingValue;
				OwnerId = _startingOwnerId;
				PlayerRef = OwnerId == -1 ? default : _lobbyService.PlayerRefToIdMap.First(kp => kp.Value == OwnerId).Key;
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