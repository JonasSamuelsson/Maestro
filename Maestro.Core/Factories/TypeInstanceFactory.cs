using System;
using Maestro.Utils;

namespace Maestro.Factories
{
	internal class TypeInstanceFactory : IInstanceFactory
	{
		private readonly Type _type;
		private int _configVersion;
		private Func<IContext, object> _instantiator;

		public TypeInstanceFactory(Type type)
		{
			_type = type;
			_configVersion = Int32.MinValue;
		}

		public bool CanGet(IContext context)
		{
			var instantiator = _instantiator;

			if (_configVersion != context.ConfigVersion)
			{
				instantiator = Reflector.GetInstantiatorOrNull(_type, context);
				_instantiator = instantiator;
				_configVersion = context.ConfigVersion;
			}

			return instantiator != null;
		}

		public object Get(IContext context)
		{
			var instantiator = _instantiator;

			if (_configVersion != context.ConfigVersion)
			{
				instantiator = Reflector.GetInstantiatorOrNull(_type, context);
				_instantiator = instantiator;
				_configVersion = context.ConfigVersion;
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

		public IInstanceFactory Clone()
		{
			return new TypeInstanceFactory(_type);
		}

		public override string ToString()
		{
			return string.Format("type instance : {0}", _type);
		}
	}
}