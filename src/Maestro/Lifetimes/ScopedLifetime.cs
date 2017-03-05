using System;
using System.Collections.Generic;

namespace Maestro.Lifetimes
{
	internal class ScopedLifetime : ILifetime
	{
		private readonly Dictionary<IContext, object> _dictionary = new Dictionary<IContext, object>();

		public object Execute(IContext context, Func<IContext, object> factory)
		{
			object instance;

			lock (_dictionary)
				if (_dictionary.TryGetValue(context, out instance))
					return instance;

			instance = factory(context);
			context.Disposed += ContextOnDisposed;
			lock (_dictionary) _dictionary.Add(context, instance);
			return instance;
		}

		private void ContextOnDisposed(IContext context)
		{
			lock (_dictionary) _dictionary.Remove(context);
		}

		public override string ToString()
		{
			return "Scoped";
		}
	}
}