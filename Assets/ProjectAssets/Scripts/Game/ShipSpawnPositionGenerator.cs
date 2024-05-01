using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class ShipSpawnPositionGenerator : MonoBehaviour
    {
        [SerializeField] private float _radiusOffset;
        [SerializeField] private GameObject _parentPlanet;
        [SerializeField] private int _layers = 3;
        
        private float _planetRadius;
        private float _shipRadius;
        private GameObject _shipPrefab;
        private float _layerSpacing;

        public int MaxShipsSpawn { get; private set; }

        public void Initialize(GameObject shipObject)
        {
            _shipPrefab = shipObject;
            _planetRadius = GetObjectRadius(_parentPlanet);
            _shipRadius = GetObjectRadius(_shipPrefab);
            _layerSpacing = Mathf.Sqrt(3) * _shipRadius;
            float innermostSpawnRadius = _planetRadius + _radiusOffset + _shipRadius;
            float shipArc = 2 * Mathf.Asin(_shipRadius / innermostSpawnRadius);
            MaxShipsSpawn = (int)(2 * Mathf.PI / shipArc) * _layers;
        }

        public IEnumerable<Vector3> CalculateShipSpawnPositions(GameObject targetPlanet, int numberOfShips)
        {
            float spawnRadius = _planetRadius + _radiusOffset + _shipRadius;
            var directionToPlanet = (targetPlanet.transform.position - _parentPlanet.transform.position).normalized;
            var rotationToPlanet = Quaternion.FromToRotation(Vector3.right, directionToPlanet);

            float shipArc = 2 * Mathf.Asin(_shipRadius / spawnRadius);
            int maxShipsPerLayer = (int)(2 * Mathf.PI / shipArc);

            int remainingShipsToSpawn = numberOfShips;
            int currentLayer = 0;

            while (remainingShipsToSpawn > 0)
            {
                float currentRadius = spawnRadius + currentLayer * _layerSpacing;
                int shipsInThisLayer = Mathf.Min(maxShipsPerLayer, remainingShipsToSpawn);
                float layerAngleOffset = (currentLayer % 2) * (Mathf.PI / maxShipsPerLayer);

                float coveredArc = shipsInThisLayer * shipArc;
                float initialAngle = -coveredArc / 2;

                for (int i = 0; i < shipsInThisLayer; i++)
                {
                    float angleInRadians = initialAngle + i * shipArc + layerAngleOffset;
                    var spawnPosition = CalculatePositionOnCircle(currentRadius, angleInRadians, rotationToPlanet);
                    yield return spawnPosition;
                }

                remainingShipsToSpawn -= shipsInThisLayer;
                ++currentLayer;
            }
        }
        
        private Vector3 CalculatePositionOnCircle(float radius, float angleInRadians, Quaternion rotation)
        {
            float x = Mathf.Cos(angleInRadians) * radius;
            float y = Mathf.Sin(angleInRadians) * radius;
            return _parentPlanet.transform.position + rotation * new Vector3(x, y, 0);
        }

        private float GetObjectRadius(GameObject planet)
        {
            var collider2d = planet.GetComponent<CircleCollider2D>();
            return collider2d.radius * planet.transform.localScale.x;
        }
    }
}