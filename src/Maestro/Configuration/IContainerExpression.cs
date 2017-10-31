using System;
using System.Collections.Generic;

namespace Maestro.Configuration
{
	public interface IContainerExpression
	{
		IConfigExpression Config { get; }
		IList<ITypeProvider> TypeProviders { get; }

		IServiceExpression Use(Type type);
		IServiceExpression Use(Type type, string name);
		IServiceExpression<TService> Use<TService>();
		IServiceExpression<TService> Use<TService>(string name);

		IServiceExpression TryUse(Type type);
		IServiceExpression TryUse(Type type, string name);
		IServiceExpression<TService> TryUse<TService>();
		IServiceExpression<TService> TryUse<TService>(string name);

		IServiceExpression Add(Type type);
		IServiceExpression<TService> Add<TService>();
		void Scan(Action<IScanner> action);
	}
}