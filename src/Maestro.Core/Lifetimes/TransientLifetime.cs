using System;

namespace Maestro.Lifetimes
{
	internal class TransientLifetime : ILifetime
	{
		static TransientLifetime()
		{
			Instance = new TransientLifetime();
		}

		public static ILifetime Instance { get; }

		public object Execute(INextStep nextStep)
		{
			return nextStep.Execute();
		}

		public ILifetime MakeGeneric(Type[] genericArguments)
		{
			return Instance;
		}

		public override string ToString()
		{
			return "transient";
		}
	}
}