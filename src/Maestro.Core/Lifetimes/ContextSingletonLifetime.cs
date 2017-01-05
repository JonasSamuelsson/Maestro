using System;
using System.Collections.Generic;
using Maestro.Internals;

namespace Maestro.Lifetimes
{
	internal class ContextSingletonLifetime : ILifetime
	{
		private readonly Dictionary<Context, object> _dictionary;

		public ContextSingletonLifetime()
		{
			_dictionary = new Dictionary<Context, object>();
		}

		public object Execute(IContext context, Func<IContext, object> factory)
		{
			object instance;
			var ctx = (Context)context;

			lock (_dictionary)
				if (_dictionary.TryGetValue(ctx, out instance))
					return instance;

			instance = factory(context);
			ctx.Disposed += ContextOnDisposed;
			lock (_dictionary) _dictionary.Add(ctx, instance);
			return instance;
		}

		public ILifetime MakeGeneric(Type[] genericArguments)
		{
			return new ContextSingletonLifetime();
		}

		private void ContextOnDisposed(Context context)
		{
			lock (_dictionary) _dictionary.Remove(context);
		}

		public override string ToString()
		{
			return "context";
		}
	}
}