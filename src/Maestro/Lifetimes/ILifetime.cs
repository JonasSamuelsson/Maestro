using System;

namespace Maestro.Lifetimes
{
	public interface ILifetime
	{
		object Execute(Context context, Func<Context, object> factory);
	}
}