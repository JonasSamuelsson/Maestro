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

		public object Execute(Context context, Func<Context, object> factory)
		{
			return factory(context);
		}

		public ILifetime MakeGeneric(Type[] genericArguments)
		{
			return Instance;
		}

		public override string ToString()
		{
			return "Transient";
		}
	}
}