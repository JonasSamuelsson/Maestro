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

		public ILifetime Clone()
		{
			return new ContextSingletonLifetime();
		}

		public object Execute(INextStep nextStep)
		{
			object instance;
			var context = ((Pipeline.NextStep)nextStep).Context;

			lock (_dictionary)
			if (_dictionary.TryGetValue(context, out instance))
					return instance;

			instance = nextStep.Execute();
			context.Disposed += ContextOnDisposed;
			lock (_dictionary) _dictionary.Add(context, instance);
			return instance;
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