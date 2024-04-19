using System;

namespace Game.Abstractions
{
	public interface IPlanetSwipeDetector
	{
		public IObservable<(Planet startPlanet, Planet hoverPlanet)> OnPlanetHover { get; }
		public IObservable<(Planet startPlanet, Planet hoverPlanet)> OnPlanetStopHover { get; }
		public IObservable<Planet> OnPlanetStartSelect { get; }
		public IObservable<(Planet startPlanet, Planet endPlanet)> OnPlanetSwipe { get; }
	}
}