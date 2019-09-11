using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Maestro.FactoryProviders.Factories;
using Maestro.Internals;

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
			var activator = GetActivator(context);
			return new TypeFactory(Type, activator);
		}

		private Func<Context, object> GetActivator(Context context)
		{
			var constructors = (Constructor != null ? new[] { Constructor } : ConstructorProvider.GetConstructors(Type))
				.ToList();

			foreach (var constructor in constructors)
			{
				var parameters = constructor.GetParameters();
				var parameterFactories = new List<Func<Context, object>>();

				try
				{
					foreach (var parameter in parameters)
					{
						if (!TryGetParameterFactory(parameter, context, out var factory))
							break;

						parameterFactories.Add(factory);
					}

					if (parameterFactories.Count == parameters.Length)
					{
						return CreateActivator(constructor, parameterFactories);
					}
				}
				catch (ActivationException exception)
				{
					GetTraceFrameInfos(constructor).ForEach(exception.AddTraceFrameInfo);
					throw;
				}
			}

			var message = $"Can't find resolvable constructor for '{Type.ToFormattedString()}'.";
			var traceFrameInfos = GetTraceFrameInfos(constructors);
			throw new InstantiationException(message, traceFrameInfos);
		}

		private bool TryGetParameterFactory(ParameterInfo parameter, Context context, out Func<Context, object> factory)
		{
			factory = null;

			var parameterType = parameter.ParameterType;

			var customFactory = CtorArgs.SingleOrDefault(ca => ca.Type == parameterType)?.Factory;
			if (customFactory != null)
			{
				factory = ctx => customFactory(ctx, parameterType);
				return true;
			}

			if (context.TryGetPipeline(parameterType, Name, out var pipeline))
			{
				factory = ctx => ctx.ExecutePipeline(pipeline, parameterType, Name);
				return true;
			}

			if (parameter.IsOptional)
			{
				var defaultValue = parameter.DefaultValue;
				factory = _ => defaultValue;
				return true;
			}

			return false;
		}

		private static IEnumerable<string> GetTraceFrameInfos(ConstructorInfo constructor)
		{
			yield return $"implementation type: {constructor.DeclaringType.ToFormattedString()}";

			var parameterTypes = constructor.GetParameters().Select(x => x.ParameterType.ToFormattedString());
			yield return $"constructor parameters: {string.Join(", ", parameterTypes)}";
		}

		private static IEnumerable<string> GetTraceFrameInfos(IEnumerable<ConstructorInfo> constructors)
		{
			constructors = constructors.ToList();

			yield return $"implementation type: {constructors.First().DeclaringType.ToFormattedString()}";

			var counter = 1;
			foreach (var constructor in constructors)
			{
				var parameterTypes = constructor.GetParameters().Select(x => x.ParameterType.ToFormattedString());
				yield return $"candidate {counter++} parameters: {string.Join(", ", parameterTypes)}";
			}
		}

		private Func<Context, object> CreateActivator(ConstructorInfo constructor, List<Func<Context, object>> parameterFactories)
		{
			var constructorAdapter = ConstructorAdapterFactory.Create(constructor, parameterFactories);
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