using System;

namespace Maestro.Lifetimes
{
	internal class TransientLifetime : Lifetime
	{
		static TransientLifetime()
		{
			Instance = new TransientLifetime();
		}

		public static Lifetime Instance { get; }

		public override object Execute(Context context, Func<Context, object> factory)
		{
			return factory(context);
		}

		public override Lifetime MakeGeneric(Type[] genericArguments)
		{
			return Instance;
		}

		public override string ToString()
		{
			return "Transient";
		}
	}
}