using System;
using UniRx;
using Zenject;

namespace Abstractions
{
	public abstract class AService : IInitializable, IDisposable
	{
		protected CompositeDisposable Disposer { get; private set; }
		
		public virtual void Initialize()
		{
			Disposer = new CompositeDisposable();
		}
		
		public virtual void Dispose()
		{
			Disposer?.Dispose();
		}
	}
}