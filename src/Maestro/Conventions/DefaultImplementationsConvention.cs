using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Configuration;

namespace Maestro.Conventions
{
	internal class DefaultImplementationsConvention : IConvention
	{
		private readonly ServiceRegistration _registration;
		private readonly Action<ITypeInstanceExpression<object>> _instanceConfiguration;

		public DefaultImplementationsConvention(ServiceRegistration registration, Action<ITypeInstanceExpression<object>> instanceConfiguration)
		{
			_registration = registration;
			_instanceConfiguration = instanceConfiguration;
		}

		public void Process(IEnumerable<Type> types, IContainerExpression containerExpression)
		{
			types = types as IList<Type> ?? types.ToList();

			var interfaces = types.Where(x => x.IsInterface);
			var classes = types.Where(x => x.IsConcreteClass()).GroupBy(x => x.Namespace ?? string.Empty).ToDictionary(x => x.Key, x => x.ToList());

			foreach (var @interface in interfaces)
			{
				List<Type> list;
				if (!classes.TryGetValue(@interface.Namespace ?? string.Empty, out list)) continue;
				var @class = list.SingleOrDefault(x => x.Name == @interface.Name.Substring(1));
				if (@class == null) continue;
				var typeInstanceExpression = _registration.Invoke(@interface, @class);
				if (typeInstanceExpression == null) continue;
				_instanceConfiguration?.Invoke(typeInstanceExpression);
			}
		}
	}
}