using System;
using Maestro.Utils;

namespace Maestro.Lifetimes
{
	public class ContainerSingletonLifetime : ILifetime
	{
		private readonly ThreadSafeDictionary<Guid, object> _cache = new ThreadSafeDictionary<Guid, object>();

		public ILifetime Clone()
		{
			return new ContainerSingletonLifetime();
		}

		public object Execute(IContext context, IPipeline pipeline)
		{
			object instance;
			if (!_cache.TryGet(context.ContainerId, out instance))
				lock (this)
					if (!_cache.TryGet(context.ContainerId, out instance))
					{
						instance = pipeline.Execute();
						_cache.Add(context.ContainerId, instance);
						context.ContainerDisposed += ContainerDisposed;
					}

			return instance;
		}

		private void ContainerDisposed(Guid containerId)
		{
			lock (this)
			{
				object instance;
				if (!_cache.TryGet(containerId, out instance)) return;
				_cache.Remove(containerId);
				var disposable = instance as IDisposable;
				if (disposable == null) return;
				disposable.Dispose();
			}
		}
	}
}