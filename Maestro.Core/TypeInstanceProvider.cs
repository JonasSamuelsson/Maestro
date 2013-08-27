using System;
using System.Linq;
using System.Reflection;

namespace Maestro
{
	internal class TypeInstanceProvider : IProvider
	{
		private readonly Type _type;
		private int _configId;
		private bool? _canGet;
		private Func<object[], object> _ctor;
		private Type[] _ctorParameterTypes;

		public TypeInstanceProvider(Type type)
		{
			_type = type;
		}

		public bool CanGet(IContext context)
		{
			if (ShouldReevaluateSelectedCtor(context))
				FindCtor(context);

			return _canGet.Value;
		}

		private bool ShouldReevaluateSelectedCtor(IContext context)
		{
			return !_canGet.HasValue || _configId != context.ConfigId;
		}

		private void FindCtor(IContext context)
		{
			_configId = context.ConfigId;

			ConstructorInfo constructor;
			if (!TryGetConstructor(context, out constructor))
			{
				_canGet = false;
				_ctorParameterTypes = null;
				return;
			}

			_ctor = Reflector.GetInstantiator(constructor);
			_ctorParameterTypes = constructor.GetParameters().Select(x => x.ParameterType).ToArray();
			_canGet = true;
		}

		private bool TryGetConstructor(IContext context, out ConstructorInfo constructor)
		{
			constructor = null;
			foreach (var ctor in _type.GetConstructors().OrderByDescending(x => x.GetParameters().Length))
			{
				var parameterTypes = ctor.GetParameters().Select(x => x.ParameterType).ToList();
				if (parameterTypes.Any() && !parameterTypes.All(context.CanGet)) continue;
				constructor = ctor;
				return true;
			}
			return false;
		}

		public object Get(IContext context)
		{
			if (ShouldReevaluateSelectedCtor(context))
				FindCtor(context);

			if (!_canGet.Value)
				throw new InvalidOperationException(string.Format("Can't find appropriate constructor to invoke {0}.", _type.FullName));

			var ctorArgs = _ctorParameterTypes.Select(context.Get).ToArray();
			return _ctor(ctorArgs);
		}

		public IProvider MakeGenericProvider(Type[] types)
		{
			var genericType = _type.MakeGenericType(types);
			return new TypeInstanceProvider(genericType);
		}
	}
}