using System;

namespace Maestro
{
	internal class TypeInstanceProvider : IProvider
	{
		private readonly Type _type;
		private int _configId;
		private Func<IContext, object> _instantiator;

		public TypeInstanceProvider(Type type)
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

		//private bool ShouldReevaluateSelectedCtor(IContext context)
		//{
		//	return !_canGet.HasValue || _configId != context.ConfigId;
		//}

		//private void FindCtor(IContext context)
		//{
		//	_configId = context.ConfigId;

		//	ConstructorInfo constructor;
		//	if (!TryGetConstructor(context, out constructor))
		//	{
		//		_canGet = false;
		//		_ctorParameterTypes = null;
		//		return;
		//	}

		//	_ctor = Reflector.GetConstructorCall(constructor);
		//	_ctorParameterTypes = constructor.GetParameters().Select(x => x.ParameterType).ToArray();
		//	_canGet = true;
		//}

		//private bool TryGetConstructor(IContext context, out ConstructorInfo constructor)
		//{
		//	constructor = null;
		//	foreach (var ctor in _type.GetConstructors().OrderByDescending(x => x.GetParameters().Length))
		//	{
		//		var parameterTypes = ctor.GetParameters().Select(x => x.ParameterType).ToList();
		//		if (parameterTypes.Any() && !parameterTypes.All(context.CanGet)) continue;
		//		constructor = ctor;
		//		return true;
		//	}
		//	return false;
		//}

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

		public IProvider MakeGenericProvider(Type[] types)
		{
			var genericType = _type.MakeGenericType(types);
			return new TypeInstanceProvider(genericType);
		}
	}
}