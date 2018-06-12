using Maestro.Internals;

namespace Maestro.Diagnostics
{
	public class Diagnostics
	{
		private readonly Kernel _kernel;

		internal Diagnostics(Kernel kernel)
		{
			_kernel = kernel;
		}

		public string WhatDoIHave()
		{
			var config = new Configuration();
			_kernel.Populate(config);
			return config.ToString();
		}
	}
}