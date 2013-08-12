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
		private Type[] _ctorParameterTypes;

		public TypeInstanceProvider(Type type)
		{
			_type = type;
		}

		public bool CanGet(IContext context)
		{
			if (!_canGet.HasValue || _configId != context.ConfigId)
				FindCtor(context);

			return _canGet.HasValue;
		}

		private void FindCtor(IContext context)
		{
			ConstructorInfo constructor;
			if (!ConstructorResolver.TryGetConstructor(_type, context, out constructor))
			{
				_canGet = false;
				_ctorParameterTypes = null;
				return;
			}

			_ctorParameterTypes = constructor.GetParameters().Select(x => x.ParameterType).ToArray();
			_canGet = true;
		}

		public object Get(IContext context)
		{
			if (!_canGet.HasValue || _configId != context.ConfigId)
				FindCtor(context);

			if (!_canGet.Value)
				throw new InvalidOperationException(string.Format("Can't find appropriate constructor to invoke {0}.", _type.FullName));

			var ctorArgs = _ctorParameterTypes.Select(context.Get).ToArray();
			return Activator.CreateInstance(_type, ctorArgs);
		}
	}
}