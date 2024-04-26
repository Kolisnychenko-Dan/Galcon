using System;
using Fusion;
using Game.Abstractions;
using Zenject;

namespace Game
{
	public class AttackController : NetworkBehaviour
	{
		[Inject] private IPlanetSwipeDetector _planetSwipeDetector;

		private void Start()
		{
			
		}
	}
}