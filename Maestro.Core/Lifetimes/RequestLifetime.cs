
using System;
using System.Collections.Generic;

namespace Maestro.Lifetimes
{
	public class RequestLifetime : ILifetime
	{
		private readonly string _lockKey = Guid.NewGuid().ToString();
		private readonly Dictionary<long, object> _dictionary = new Dictionary<long, object>();

		public ILifetime Clone()
		{
			return new RequestLifetime();
		}

		public object Execute(IContext context, IPipeline pipeline)
		{
			lock (_dictionary)
			{
				object instance;
				if (_dictionary.TryGetValue(context.RequestId, out instance))
					return instance;
			}

			lock (_lockKey)
			{
				var instance = pipeline.Execute();
				lock (_dictionary)
				{
					_dictionary.Add(context.RequestId, instance);
					context.Disposed += () => Remove(context.RequestId);
					return instance;
				}
			}
		}

		private void Remove(long requestId)
		{
			lock (_dictionary)
				_dictionary.Remove(requestId);
		}
	}
}