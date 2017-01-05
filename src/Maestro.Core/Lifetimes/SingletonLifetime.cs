using System;

namespace Maestro.Lifetimes
{
	internal class SingletonLifetime : ILifetime
	{
		private object _instance;

		public object Execute(IContext context, Func<IContext, object> factory)
		{
			if (_instance == null)
				lock (this)
					if (_instance == null)
						_instance = factory(context);

			return _instance;
		}

		public ILifetime MakeGeneric(Type[] genericArguments)
		{
			return new SingletonLifetime();
		}

		public override string ToString()
		{
			return "singleton";
		}
	}
}