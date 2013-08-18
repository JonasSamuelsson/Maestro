using System;

namespace Maestro
{
	internal class ConstantInstanceProvider : IProvider
	{
		private readonly object _instance;

		public ConstantInstanceProvider(object instance)
		{
			_instance = instance;
		}

		public bool CanGet(IContext context)
		{
			return true;
		}

		public object Get(IContext context)
		{
			return _instance;
		}

		public IProvider MakeGenericProvider(Type[] types)
		{
			throw new NotSupportedException();
		}
	}
}