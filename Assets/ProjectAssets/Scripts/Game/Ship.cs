using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.LagCompensation;
using UnityEngine;

namespace Game
{
	public class Ship : NetworkBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private LayerMask _planetCollisionLayer;
        [SerializeField] private Rigidbody2D _rigidbody;
        
        [Networked] public Vector3 TargetPosition { get; set; }
        
        private List<LagCompensatedHit> _lagCompensatedHits = new ();
        private float _shipCollisionRadius = 1.0f;
        private int _ownerId;
        private Planet _destinationPlanet;
        
        public void Initialize(Planet destinationPlanet, int ownerId)
        {
            if (Runner.IsServer)
            {
                TargetPosition = destinationPlanet.transform.position;
            }

            _ownerId = ownerId;
            _destinationPlanet = destinationPlanet;
        }
        
        private void Update()
        {
            if (Runner.IsServer)
            {
                MoveTowardsTarget();
                CheckForCollision();
            }
        }

        private void MoveTowardsTarget()
        {
            var targetPosition = new Vector2(TargetPosition.x, TargetPosition.y);
            _rigidbody.position = Vector2.MoveTowards(_rigidbody.position, targetPosition, Time.deltaTime * speed);

            var directionToTarget = (Vector3)targetPosition - transform.position;
            if (directionToTarget != Vector3.zero)
            {
                float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg - 90f;
                _rigidbody.rotation = angle;
            }
        }

        private void CheckForCollision()
        {
            _lagCompensatedHits.Clear();
            
            Runner.LagCompensation.OverlapSphere(_rigidbody.position, _shipCollisionRadius,
                Object.InputAuthority, _lagCompensatedHits,
                _planetCollisionLayer.value);
            
            if (_lagCompensatedHits.Any(hit => _planetCollisionLayer == (_planetCollisionLayer | (1 << hit.GameObject.layer))
                && hit.GameObject.GetComponent<Planet>() == _destinationPlanet))
            {
                Debug.Log("tryDespawn");
                DestroyShip();
            }
        }
        
        private void DestroyShip()
        {
            Runner.Despawn(Object);
        }
    }
}