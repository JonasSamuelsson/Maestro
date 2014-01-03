using System;
using Maestro.Utils;

namespace Maestro.Interceptors
{
	internal class SetPropertyInterceptor : IInterceptor
	{
		private readonly string _propertyName;
		private readonly Func<IContext, object> _factory;
		private Setter _setter;

		public SetPropertyInterceptor(string propertyName, Func<IContext, object> factory = null)
		{
			_propertyName = propertyName;
			_factory = factory;
		}

		public IInterceptor Clone()
		{
			return new SetPropertyInterceptor(_propertyName);
		}

		public object Execute(object instance, IContext context)
		{
			var instanceType = instance.GetType();
			var propertyType = instanceType.GetProperty(_propertyName).PropertyType;

			var setter = _setter;
			if (setter == null || setter.ConfigVersion != context.ConfigVersion)
			{
				setter = new Setter
				         {
					         ConfigVersion = context.ConfigVersion,
					         Get = _factory ?? Reflector.GetPropertyValueProvider(propertyType, context),
					         Set = Reflector.GetPropertySetter(instanceType, _propertyName)
				         };
				_setter = setter;
			}

			setter.Set(instance, setter.Get(context));
			return instance;
		}

		public override string ToString()
		{
			return string.Format("set property {0}", _propertyName);
		}

		private class Setter
		{
			public int ConfigVersion { get; set; }
			public Func<IContext, object> Get { get; set; }
			public Action<object, object> Set { get; set; }
		}
	}
}