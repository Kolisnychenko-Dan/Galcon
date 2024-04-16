using System;
using System.Collections.Generic;
using Abstractions;
using UniRx;

namespace Tools
{
	public class EventManager : AService, IEventManager
	{
		private readonly Dictionary<string, object> _paramEvents = new();

		public static IEventManager Instance { get; private set; }
		
		public override void Initialize()
		{
			base.Initialize();
			Instance = this;
		}

		public IObservable<T> GetEvent<T>(string eventName)
		{
			if (!_paramEvents.TryGetValue(eventName, out object eventObj))
			{
				var newEvent = new Subject<T>();
				_paramEvents[eventName] = newEvent;
				return newEvent;
			}

			return (Subject<T>) eventObj;
		}

		public void EmitEvent<T>(string eventName, T data)
		{
			if (_paramEvents.TryGetValue(eventName, out object eventObj))
			{
				((Subject<T>)eventObj).OnNext(data);
			}
		}

		public void EmitEvent(string eventName)
		{
			EmitEvent(eventName, Unit.Default);
		}
	}
}