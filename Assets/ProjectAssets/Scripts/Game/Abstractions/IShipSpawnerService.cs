using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Game.Abstractions
{
	public interface IShipSpawnerService
	{
		UniTask<List<Ship>> SpawnShips(int ownerId, int amount, Planet parentPlanet, Planet toPlanet);
	}
}