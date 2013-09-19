using System;

namespace Maestro.Interceptors
{
	internal class TrySetPropertyInterceptor : IInterceptor
	{
		private readonly string _propertyName;
		private Action<object, object> _setter;

		public TrySetPropertyInterceptor(string propertyName)
		{
			_propertyName = propertyName;
		}

		public IInterceptor Clone()
		{
			return new TrySetPropertyInterceptor(_propertyName);
		}

		public object Execute(object instance, IContext context)
		{
			var instanceType = instance.GetType();
			var propertyType = instanceType.GetProperty(_propertyName).PropertyType;

			if (context.CanGet(propertyType))
			{
				if (_setter == null)
					_setter = Reflector.GetPropertySetter(instanceType, _propertyName);

				var value = context.Get(propertyType);
				_setter(instance, value);
			}

			return instance;
		}

		public override string ToString()
		{
			return string.Format("try set property {0}", _propertyName);
		}
	}
}