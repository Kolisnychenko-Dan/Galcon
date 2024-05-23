using System.Collections.Generic;
using UniRx;

namespace Game.Abstractions
{
	public interface IMapSpawnerService
	{
		void SpawnMap(int playerCount);
		IReactiveProperty<IReadOnlyList<Planet>> Planets { get; }
	}
}