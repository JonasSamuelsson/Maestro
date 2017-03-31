using System;
using Maestro.Internals;

namespace Maestro.Diagnostics
{
	public class Diagnostics : IDiagnostics
	{
		private readonly Kernel _kernel;

		internal Diagnostics(Kernel kernel)
		{
			_kernel = kernel;
		}

		public string WhatDoIHave(Func<Type, bool> predicate = null)
		{
			var config = new Configuration();
			_kernel.Populate(config);
			return config.ToString(predicate ?? (_ => true));
		}
	}
}