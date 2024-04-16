using System;

namespace Tools
{
	public interface IEventManager
	{
		IObservable<T> GetEvent<T>(string eventName);
		void EmitEvent<T>(string eventName, T data);
		void EmitEvent(string eventName);
	}
}