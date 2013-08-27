using System;

namespace Maestro.Interceptors
{
	internal class SetPropertyInterceptor : IInterceptor
	{
		private readonly string _propertyName;
		private Action<object, object> _setter;

		public SetPropertyInterceptor(string propertyName)
		{
			_propertyName = propertyName;
		}

		public IInterceptor Clone()
		{
			return new SetPropertyInterceptor(_propertyName);
		}

		public object Execute(object instance, IContext context)
		{
			var instanceType = instance.GetType();
			var propertyType = instanceType.GetProperty(_propertyName).PropertyType;

			if (_setter == null)
				_setter = Reflector.GetPropertySetter(instanceType, _propertyName);

			var value = context.Get(propertyType);
			_setter(instance, value);
			return instance;
		}
	}
}