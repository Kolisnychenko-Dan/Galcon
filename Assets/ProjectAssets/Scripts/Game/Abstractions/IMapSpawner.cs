using System.Collections.Generic;
using UniRx;

namespace Game.Abstractions
{
	public interface IMapSpawner
	{
		void SpawnPlanets(int playerCount);
		IReactiveProperty<IReadOnlyList<Planet>> Planets { get; }
	}
}