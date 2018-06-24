using Maestro.FactoryProviders.Factories;
using Maestro.Internals;
using Maestro.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maestro.FactoryProviders
{
	class TypeFactoryProvider : IFactoryProvider
	{
		private static readonly ConstructorProvider ConstructorProvider = new ConstructorProvider();

		public TypeFactoryProvider(Type type, string name)
		{
			Type = type;
			Name = name;
		}

		public Type Type { get; }
		public string Name { get; }
		public ConstructorInfo Constructor { get; set; }
		public IList<CtorArg> CtorArgs { get; private set; } = new List<CtorArg>();

		public Factory GetFactory(Context context)
		{
			var activator = GetActivator(Constructor, Name, context);
			return new TypeFactory(Type, activator);
		}

		private Func<Context, object> GetActivator(ConstructorInfo constructor, string name, Context context)
		{
			var ctors = from ctor in constructor != null ? new[] { constructor } : ConstructorProvider.GetConstructors(Type)
							let parameters = ctor.GetParameters()
							select new { ctor, parameters };

			foreach (var x in ctors)
			{
				var factories = new List<Func<Context, object>>();

				foreach (var parameter in x.parameters)
				{
					var parameterType = parameter.ParameterType;

					var customFactory = CtorArgs.SingleOrDefault(ca => ca.Type == parameterType)?.Factory;
					if (customFactory != null)
					{
						factories.Add(ctx => customFactory(ctx, parameterType));
						continue;
					}

					if (context.TryGetPipeline(parameterType, name, out var pipeline))
					{
						factories.Add(ctx => ctx.GetService(parameterType, name, pipeline));
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
							var error = $"Error instantiating '{Type.ToFormattedString()}'.";
							throw new InvalidOperationException(ExceptionMessageBuilder.GetMessage(error, exception.Message));
						}
					};
				}
			}

			var message = $"Could not find resolvable constructor for '{Type.ToFormattedString()}'.";
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
			public Type Type { get; set; }
			public Func<Context, Type, object> Factory { get; set; }
		}
	}
}