using System;

namespace Maestro.Interceptors
{
	internal class SetPropertyInterceptor : Interceptor<object>
	{
		// todo : perf

		private readonly string _propertyName;
		private readonly string _name;
		private Func<IContext, object> _valueFactory;
		private SetPropertyAction _setPropertyAction;

		public SetPropertyInterceptor(string propertyName, string name)
		{
			_propertyName = propertyName;
			_name = name;
		}

		public SetPropertyInterceptor(string propertyName, Func<IContext, object> valueFactory)
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
					_valueFactory = GetValueFactory(property.PropertyType, _name);
				}

				_setPropertyAction = SetPropertyActionFactory.Get(property);
			}

			var value = _valueFactory.Invoke(context);
			_setPropertyAction.Invoke(instance, value);
			return instance;
		}

		private static Func<IContext, object> GetValueFactory(Type propertyType, string name)
		{
			return context => context.GetService(propertyType, name);
		}

		public override IInterceptor MakeGeneric(Type[] genericArguments)
		{
			return _name != null
				? new SetPropertyInterceptor(_propertyName, _name)
				: new SetPropertyInterceptor(_propertyName, _valueFactory);
		}

		public override string ToString()
		{
			return $"set property {_propertyName}";
		}
	}
}