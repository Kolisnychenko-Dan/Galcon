using System;
using System.Collections.Generic;
using Fusion;
using Game.Abstractions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
	public class PlanetSpawner : NetworkBehaviour, IPlanetSpawner
	{
		[SerializeField] private NetworkPrefabRef _planet;

		private float _screenBoundaryX = 0.0f;
		private float _screenBoundaryY = 0.0f;

		private List<NetworkId> _planets = new();

		public override void Spawned()
		{
			base.Spawned();
			
			SpawnPlanets(0);
		}

		public void SpawnPlanets(int playerCount)
		{
			if (Object.HasStateAuthority == false) return;
			
			_screenBoundaryX = Camera.main.orthographicSize * Camera.main.aspect;
			_screenBoundaryY = Camera.main.orthographicSize;
			
			SpawnPlanet();
		}
		
		private void SpawnPlanet()
		{
			var direction = Random.insideUnitCircle;
			var position = Vector3.zero;

			if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
			{
				position = new Vector3(Mathf.Sign(direction.x) * _screenBoundaryX, direction.y * _screenBoundaryY, 0);
			}
			else
			{
				position = new Vector3(direction.x * _screenBoundaryX, Mathf.Sign(direction.y) * _screenBoundaryY, 0);
			}

			position -= position.normalized * 0.1f;

			var rotation = Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f),
				Random.Range(0.0f, 360.0f));

			var planet = Runner.Spawn(_planet, position, rotation, PlayerRef.None,
				onBeforeSpawned: (_,_) => {});

			_planets.Add(planet.Id);
		}
	}
}