using System;
using Maestro.FactoryProviders;
using Maestro.Internals;

namespace Maestro.Configuration
{
	class TypeInstanceConfigurator<TInstance> : InstanceConfigurator<TInstance, ITypeInstanceConfigurator<TInstance>>, ITypeInstanceConfigurator<TInstance>
	{
		public TypeInstanceConfigurator(ServiceDescriptor serviceDescriptor)
			: base(serviceDescriptor)
		{
		}

		internal override ITypeInstanceConfigurator<TInstance> Parent => this;

		public ITypeInstanceConfigurator<TInstance> CtorArg(string argName, object value)
		{
			return CtorArg(argName, (ctx, type) => value);
		}

		public ITypeInstanceConfigurator<TInstance> CtorArg(string argName, Func<object> factory)
		{
			return CtorArg(argName, (ctx, type) => factory());
		}

		public ITypeInstanceConfigurator<TInstance> CtorArg(string argName, Func<IContext, object> factory)
		{
			return CtorArg(argName, (ctx, type) => factory(ctx));
		}

		public ITypeInstanceConfigurator<TInstance> CtorArg(string argName, Func<IContext, Type, object> factory)
		{
			var typeFactoryProvider = (TypeFactoryProvider)ServiceDescriptor.FactoryProvider;
			var ctorArg = new TypeFactoryProvider.CtorArg { Name = argName, Factory = factory };
			typeFactoryProvider.CtorArgs.Add(ctorArg);
			return this;
		}

		public ITypeInstanceConfigurator<TInstance> CtorArg(Type argType, object value)
		{
			return CtorArg(argType, (ctx, type) => value);
		}

		public ITypeInstanceConfigurator<TInstance> CtorArg(Type argType, Func<object> factory)
		{
			return CtorArg(argType, (ctx, type) => factory());
		}

		public ITypeInstanceConfigurator<TInstance> CtorArg(Type argType, Func<IContext, object> factory)
		{
			return CtorArg(argType, (ctx, type) => factory(ctx));
		}

		private ITypeInstanceConfigurator<TInstance> CtorArg(Type argType, Func<IContext, Type, object> factory)
		{
			var typeFactoryProvider = (TypeFactoryProvider)ServiceDescriptor.FactoryProvider;
			var ctorArg = new TypeFactoryProvider.CtorArg { Type = argType, Factory = factory };
			typeFactoryProvider.CtorArgs.Add(ctorArg);
			return Parent;
		}

		public ITypeInstanceConfigurator<TInstance> CtorArg<TValue>(TValue value)
		{
			return CtorArg(typeof(TValue), (ctx, type) => value);
		}

		public ITypeInstanceConfigurator<TInstance> CtorArg<TValue>(Func<TValue> factory)
		{
			return CtorArg(typeof(TValue), (ctx, type) => factory());
		}

		public ITypeInstanceConfigurator<TInstance> CtorArg<TValue>(Func<IContext, TValue> factory)
		{
			return CtorArg(typeof(TValue), (ctx, type) => factory(ctx));
		}

		public ITypeInstanceConfigurator<T> As<T>()
		{
			return new TypeInstanceConfigurator<T>(ServiceDescriptor);
		}
	}
}