using System;
using Maestro.Utils;

namespace Maestro.Interceptors
{
	internal class TrySetPropertyInterceptor : IInterceptor
	{
		private readonly string _propertyName;
		private Setter _setter;

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
			var setter = _setter;
		
			if (setter == null || setter.ConfigVersion != context.ConfigVersion)
			{
				var instanceType = instance.GetType();
				var propertyType = instanceType.GetProperty(_propertyName).PropertyType;

				if (!context.CanGet(propertyType))
					return instance;

				setter = new Setter
							{
								ConfigVersion = context.ConfigVersion,
								Get = Reflector.GetPropertyValueProvider(propertyType, context),
								Set = Reflector.GetPropertySetter(instanceType, _propertyName)
							};
				_setter = setter;
			}

			setter.Set(instance, setter.Get(context));
			return instance;
		}

		public override string ToString()
		{
			return string.Format("try set property {0}", _propertyName);
		}

		private class Setter
		{
			public int ConfigVersion { get; set; }
			public Func<IContext, object> Get { get; set; }
			public Action<object, object> Set { get; set; }
		}
	}
}