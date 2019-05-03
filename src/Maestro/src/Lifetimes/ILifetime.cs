using System;

namespace Maestro.Lifetimes
{
	internal abstract class Lifetime
	{
		public abstract object Execute(Context context, Func<Context, object> factory);
		public abstract Lifetime MakeGeneric(Type[] genericArguments);
	}
}