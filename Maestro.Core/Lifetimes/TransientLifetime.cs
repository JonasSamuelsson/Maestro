using System;

namespace Maestro.Lifetimes
{
	internal class TransientLifetime : ILifetime
	{
		static TransientLifetime()
		{
			Instance = new TransientLifetime();
		}

		private TransientLifetime() { }

		public static ILifetime Instance { get; private set; }

		public ILifetime Clone()
		{
			return Instance;
		}

		public object Execute(IContext context, IPipeline pipeline)
		{
			return pipeline.Execute();
		}

		public override string ToString()
		{
			return "transient";
		}
	}
}