using Microsoft.Extensions.DependencyInjection;
using System;

namespace Maestro.Microsoft.DependencyInjection
{
	internal class MaestroServiceScope : IServiceScope
	{
		private readonly IScope _scope;

		public MaestroServiceScope(IScope scope)
		{
			_scope = scope;
		}

		public IServiceProvider ServiceProvider => _scope.ToServiceProvider();

		public void Dispose() => _scope.Dispose();
	}
}