using System;
using System.Collections.Generic;

namespace Maestro.Lifetimes
{
	internal class ContextSingletonLifetime : ILifetime
	{
		private readonly Dictionary<long, object> _dictionary;

		public ContextSingletonLifetime()
		{
			_dictionary = new Dictionary<long, object>();
		}

		public ILifetime Clone()
		{
			return new ContextSingletonLifetime();
		}

		public object Execute(IContext context, IPipeline pipeline)
		{
			throw new NotImplementedException();
			//object instance;

			//lock (_dictionary)
			//	if (_dictionary.TryGetValue(context.ContextId, out instance))
			//		return instance;

			//lock (context.ContextId.ToString())
			//{
			//	lock (_dictionary)
			//		if (_dictionary.TryGetValue(context.ContextId, out instance))
			//			return instance;

			//	instance = pipeline.Execute();

			//	lock (_dictionary)
			//	{
			//		_dictionary.Add(context.ContextId, instance);
			//		context.Disposed += () => Remove(context.ContextId);
			//		return instance;
			//	}
			//}
		}

		private void Remove(long contextId)
		{
			lock (_dictionary)
				_dictionary.Remove(contextId);
		}

		public override string ToString()
		{
			return "context";
		}
	}
}