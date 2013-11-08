using System;

namespace Maestro
{
	internal class TypeInstanceFactory : IInstanceFactory
	{
		private readonly Type _type;
		private int _configId;
		private Func<IContext, object> _instantiator;

		public TypeInstanceFactory(Type type)
		{
			_type = type;
			_configId = Int32.MinValue;
		}

		public bool CanGet(IContext context)
		{
			var instantiator = _instantiator;

			if (_configId != context.ConfigId)
			{
				instantiator = Reflector.GetInstantiatorOrNull(_type, context);
				_instantiator = instantiator;
				_configId = context.ConfigId;
			}

			return instantiator != null;
		}

		public object Get(IContext context)
		{
			var instantiator = _instantiator;

			if (_configId != context.ConfigId)
			{
				instantiator = Reflector.GetInstantiatorOrNull(_type, context);
				_instantiator = instantiator;
				_configId = context.ConfigId;
			}

			if (instantiator == null)
				throw new InvalidOperationException(string.Format("Can't find appropriate constructor for {0}.", _type.FullName));

			return instantiator.Invoke(context);
		}

		public IInstanceFactory MakeGenericInstanceFactory(Type[] types)
		{
			var genericType = _type.MakeGenericType(types);
			return new TypeInstanceFactory(genericType);
		}

		public override string ToString()
		{
			return string.Format("type instance : {0}", _type);
		}
	}
}