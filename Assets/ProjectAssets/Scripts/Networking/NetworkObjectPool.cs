using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

namespace Networking
{
	public class NetworkObjectPool : NetworkObjectProviderDefault
	{
		[SerializeField] private List<NetworkObject> _poolableObjects;

        private Dictionary<NetworkObjectTypeId, Stack<NetworkObject>> _free = new();

        protected override NetworkObject InstantiatePrefab(NetworkRunner runner, NetworkObject prefab)
        {
            if (ShouldPool(runner, prefab))
            {
                var instance = GetObjectFromPool(prefab);

                instance.transform.position = Vector3.zero;

                return instance;
            }

            return Instantiate(prefab);
        }

        protected override void DestroyPrefabInstance(NetworkRunner runner, NetworkPrefabId prefabId, NetworkObject instance)
        {
            if (_free.TryGetValue(prefabId, out var stack))
            {
                instance.gameObject.SetActive(false);
                stack.Push(instance);
            }
            else
            {
                Destroy(instance.gameObject);
            }
        }

        private NetworkObject GetObjectFromPool(NetworkObject prefab)
        {
            NetworkObject instance = null;

            if (_free.TryGetValue(prefab.NetworkTypeId, out var stack))
            {
                while (stack.Count > 0 && instance == null)
                {
                    instance = stack.Pop();
                }
            }

            if (instance == null)
                instance = GetNewInstance(prefab);

            instance.gameObject.SetActive(true);
            return instance;
        }

        private NetworkObject GetNewInstance(NetworkObject prefab)
        {
            var instance = Instantiate(prefab);

            if (_free.TryGetValue(prefab.NetworkTypeId, out var stack) == false)
            {
                stack = new Stack<NetworkObject>();
                _free.Add(prefab.NetworkTypeId, stack);
            }

            return instance;
        }

        private bool ShouldPool(NetworkRunner runner, NetworkObject prefab)
        {
            return _poolableObjects.Count == 0 || IsPoolableObject(prefab);
        }

        private bool IsPoolableObject(NetworkObject networkObject)
        {
            return _poolableObjects.Any(poolableObject => networkObject == poolableObject);
        }
	}
}