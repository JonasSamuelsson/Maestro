using System;
using Maestro.Internals;

namespace Maestro.Interceptors
{
	internal class SetPropertyInterceptor : Interceptor<object>
	{
		// todo : perf

		private readonly string _propertyName;
		private Func<IContext, object> _valueFactory;
		private SetPropertyAction _setPropertyAction;

		public SetPropertyInterceptor(string propertyName, Func<IContext, object> valueFactory = null)
		{
			_propertyName = propertyName;
			_valueFactory = valueFactory;
		}

		public override object Execute(object instance, IContext context)
		{
			var property = instance.GetType().GetProperty(_propertyName);

			if (_setPropertyAction == null)
			{
				if (_valueFactory == null)
				{
					_valueFactory = GetValueFactory(property.PropertyType);
				}

				_setPropertyAction = SetPropertyActionFactory.Get(property);
			}

			var value = _valueFactory.Invoke(context);
			_setPropertyAction.Invoke(instance, value);
			return instance;
		}

		private static Func<IContext, object> GetValueFactory(Type propertyType)
		{
			return ctx =>
					 {
						 var context = ((Context)ctx);
						 return context.GetService(propertyType);
					 };
		}

		public override IInterceptor MakeGeneric(Type[] genericArguments)
		{
			return new SetPropertyInterceptor(_propertyName, _valueFactory);
		}

		public override string ToString()
		{
			return string.Format("set property {0}", _propertyName);
		}
	}
}