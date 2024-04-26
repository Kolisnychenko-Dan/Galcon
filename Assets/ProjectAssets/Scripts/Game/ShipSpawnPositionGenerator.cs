using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class ShipPositionGeneratorSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _shipPrefab;
        [SerializeField] private float _radiusOffset;
        [SerializeField] private GameObject _parentPlanet;

        private float _planetRadius;
        private float _shipRadius;
        private int _maxShipsCount;

        public void Initialize()
        {
            _planetRadius = GetPlanetRadius(_parentPlanet);
            _shipRadius = GetShipRadius(_shipPrefab);
            float spawnRadius = _planetRadius + _radiusOffset + _shipRadius;
            float angleStep = CalculateAngleStep(spawnRadius, _shipRadius * 2);
            _maxShipsCount = (int)(2 * Mathf.PI / angleStep);
        }

        public IEnumerable<Vector3> CalculateShipSpawnPositions(GameObject targetPlanet, int numberOfShips)
        {
            numberOfShips = Mathf.Min(numberOfShips, _maxShipsCount);
            float spawnRadius = _planetRadius + _radiusOffset + _shipRadius;
            var directionToPlanet = (targetPlanet.transform.position - _parentPlanet.transform.position).normalized;
            var rotationToPlanet = Quaternion.FromToRotation(Vector3.right, directionToPlanet);

            float angleStep = CalculateAngleStep(spawnRadius, _shipRadius);
            float initialOffset = -angleStep * (numberOfShips - 1) / 2;

            for (int i = 0; i < numberOfShips; i++)
            {
                float angleInRadians = initialOffset + i * angleStep;
                var spawnPosition = CalculatePositionOnCircle(spawnRadius, angleInRadians, rotationToPlanet);
                yield return spawnPosition;
            }
        }

        private Vector3 CalculatePositionOnCircle(float radius, float angleInRadians, Quaternion rotation)
        {
            float x = Mathf.Cos(angleInRadians) * radius;
            float y = Mathf.Sin(angleInRadians) * radius;
            return _parentPlanet.transform.position + rotation * new Vector3(x, y, 0);
        }

        private float CalculateAngleStep(float radius, float shipRadius)
        {
            float arcDistance = shipRadius * 2;
            return 2 * Mathf.Asin(arcDistance / (2 * radius));
        }

        private float GetPlanetRadius(GameObject planet)
        {
            var collider2d = planet.GetComponent<CircleCollider2D>();
            return collider2d.radius * planet.transform.localScale.x;
        }

        private float GetShipRadius(GameObject shipPrefab)
        {
            var collider = shipPrefab.GetComponent<CircleCollider2D>();
            return collider.radius * shipPrefab.transform.localScale.x;
        }
    }
}