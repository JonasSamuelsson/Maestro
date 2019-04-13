using Maestro.FactoryProviders.Factories;
using Maestro.Internals;
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
				var parameterFactories = new List<Func<Context, object>>();

				foreach (var parameter in x.parameters)
				{
					var parameterType = parameter.ParameterType;

					var customFactory = CtorArgs.SingleOrDefault(ca => ca.Type == parameterType)?.Factory;
					if (customFactory != null)
					{
						parameterFactories.Add(ctx => customFactory(ctx, parameterType));
						continue;
					}

					if (context.TryGetPipeline(parameterType, name, out var pipeline))
					{
						parameterFactories.Add(ctx => ctx.GetService(parameterType, name, pipeline));
						continue;
					}

					if (parameter.IsOptional)
					{
						var defaultValue = parameter.DefaultValue;
						parameterFactories.Add(_ => defaultValue);
					}
				}

				if (parameterFactories.Count == x.parameters.Length)
				{
					var constructorAdapter = ConstructorAdapterFactory.Create(x.ctor, parameterFactories);
					return ctx =>
					{
						try
						{
							return constructorAdapter.Invoke(parameterFactories, ctx);
						}
						catch (Exception exception)
						{
							var error = $"Error instantiating '{Type.ToFormattedString()}'.";
							throw new InvalidOperationException(error, exception);
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