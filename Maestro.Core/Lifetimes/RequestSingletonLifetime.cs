using System.Collections.Generic;

namespace Maestro.Lifetimes
{
	public class RequestSingletonLifetime : ILifetime
	{
		private readonly Dictionary<long, object> _dictionary;

		public RequestSingletonLifetime()
		{
			_dictionary = new Dictionary<long, object>();
		}

		public ILifetime Clone()
		{
			return new RequestSingletonLifetime();
		}

		public object Execute(IContext context, IPipeline pipeline)
		{
			object instance;

			lock (_dictionary)
				if (_dictionary.TryGetValue(context.RequestId, out instance))
					return instance;

			lock (context.RequestId.ToString())
			{
				lock (_dictionary)
					if (_dictionary.TryGetValue(context.RequestId, out instance))
						return instance;

				instance = pipeline.Execute();

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

		public override string ToString()
		{
			return "request";
		}
	}
}