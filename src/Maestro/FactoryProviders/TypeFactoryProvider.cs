using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Maestro.FactoryProviders.Factories;
using Maestro.Internals;
using Maestro.Utils;

namespace Maestro.FactoryProviders
{
	class TypeFactoryProvider : IFactoryProvider
	{
		public TypeFactoryProvider(Type type, string name)
		{
			Type = type;
			Name = name;
		}

		public Type Type { get; }
		public string Name { get; }
		public ConstructorInfo Constructor { get; set; }
		public IList<CtorArg> CtorArgs { get; private set; } = new List<CtorArg>();

		public IFactory GetFactory(Context context)
		{
			var activator = GetActivator(Constructor, Name, context);
			return new Factory(activator);
		}

		private Func<IContext, object> GetActivator(ConstructorInfo constructor, string name, IContext context)
		{
			var ctors = from ctor in constructor != null ? new[] { constructor } : Type.GetConstructors()
							let parameters = ctor.GetParameters()
							orderby parameters.Length descending
							select new { ctor, parameters };

			foreach (var x in ctors)
			{
				var factories = new List<Func<IContext, object>>();

				foreach (var parameter in x.parameters)
				{
					var parameterType = parameter.ParameterType;

					var customFactory = CtorArgs.SingleOrDefault(ca => ca.Name == parameter.Name || ca.Type == parameterType)?.Factory;
					if (customFactory != null)
					{
						factories.Add(ctx => customFactory(ctx, parameterType));
						continue;
					}

					if (context.CanGetService(parameterType, name))
					{
						factories.Add(ctx => ctx.GetService(parameterType, name));
					}
				}

				if (factories.Count == x.parameters.Length)
				{
					var innerActivator = ConstructorInvokation.Create(x.ctor, factories);
					return ctx =>
					{
						try
						{
							return innerActivator(factories, ctx);
						}
						catch (Exception exception)
						{
							var error = $"Error instantiating '{Type.FullName}'.";
							throw new InvalidOperationException(ExceptionMessageBuilder.GetMessage(error, exception.Message));
						}
					};
				}
			}

			var message = $"Could not find resolvable constructor for '{Type.FullName}'.";
			throw new InvalidOperationException(message);
		}

		public IFactoryProvider MakeGeneric(Type[] genericArguments)
		{
			var type = Type.MakeGenericType(genericArguments);
			return new TypeFactoryProvider(type, Name) { CtorArgs = CtorArgs };
		}

		public Type GetInstanceType()
		{
			return Type;
		}

		public override string ToString()
		{
			return "Type";
		}

		internal class CtorArg
		{
			public string Name { get; set; }
			public Type Type { get; set; }
			public Func<IContext, Type, object> Factory { get; set; }
		}
	}
}