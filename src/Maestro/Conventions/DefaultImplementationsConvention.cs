using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Configuration;

namespace Maestro.Conventions
{
	internal class DefaultImplementationsConvention : IConvention
	{
		private readonly Action<IConventionalTypeInstanceExpression<object>> _action;

		public DefaultImplementationsConvention(Action<IConventionalTypeInstanceExpression<object>> action)
		{
			_action = action;
		}

		public void Process(IEnumerable<Type> types, IContainerExpression containerExpression)
		{
			types = types as IReadOnlyCollection<Type> ?? types.ToList();

			var interfaces = types.Where(x => x.IsInterface() && x.Name.StartsWith("I"));
			var classes = types
				.Where(x => x.IsConcreteClass())
				.ToDictionary(x => x.FullName);

			foreach (var @interface in interfaces)
			{
				var key = @interface.FullName;
				key = key.Remove(key.Length - @interface.Name.Length) + @interface.Name.Substring(1);
				Type @class;
				if (!classes.TryGetValue(key, out @class)) continue;
				_action(new ConventionalTypeInstanceExpression<object>(containerExpression, @interface, @class));
			}
		}
	}
}