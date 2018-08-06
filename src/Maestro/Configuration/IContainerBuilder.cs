using System;
using System.Collections.Generic;

namespace Maestro.Configuration
{
	public interface IContainerBuilder
	{
		List<Func<Type, bool>> AutoResolveFilters { get; }
		IServiceBuilder Add(Type type);
		IServiceBuilder<TService> Add<TService>();
		IServiceBuilder AddOrThrow(Type type);
		IServiceBuilder<TService> AddOrThrow<TService>();
		IServiceBuilder TryAdd(Type type);
		IServiceBuilder<TService> TryAdd<TService>();
		void Scan(Action<IScanner> action);
	}
}