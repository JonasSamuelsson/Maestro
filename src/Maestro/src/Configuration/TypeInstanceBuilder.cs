using System;
using Maestro.FactoryProviders;
using Maestro.Internals;

namespace Maestro.Configuration
{
	internal class TypeInstanceBuilder<TInstance> : InstanceBuilder<TInstance, ITypeInstanceBuilder<TInstance>>, ITypeInstanceBuilder<TInstance>
	{
		public TypeInstanceBuilder(ServiceDescriptor serviceDescriptor)
			: base(serviceDescriptor)
		{
		}

		internal override ITypeInstanceBuilder<TInstance> Parent => this;

		public ITypeInstanceBuilder<TInstance> CtorArg(Type argType, object value)
		{
			return CtorArg(argType, (ctx, type) => value);
		}

		public ITypeInstanceBuilder<TInstance> CtorArg(Type argType, Func<object> factory)
		{
			return CtorArg(argType, (ctx, type) => factory());
		}

		public ITypeInstanceBuilder<TInstance> CtorArg(Type argType, Func<Context, object> factory)
		{
			return CtorArg(argType, (ctx, type) => factory(ctx));
		}

		private ITypeInstanceBuilder<TInstance> CtorArg(Type argType, Func<Context, Type, object> factory)
		{
			var typeFactoryProvider = (TypeFactoryProvider)ServiceDescriptor.FactoryProvider;
			var ctorArg = new TypeFactoryProvider.CtorArg { Type = argType, Factory = factory };
			typeFactoryProvider.CtorArgs.Add(ctorArg);
			return Parent;
		}

		public ITypeInstanceBuilder<TInstance> CtorArg<TValue>(TValue value)
		{
			return CtorArg(typeof(TValue), (ctx, type) => value);
		}

		public ITypeInstanceBuilder<TInstance> CtorArg<TValue>(Func<TValue> factory)
		{
			return CtorArg(typeof(TValue), (ctx, type) => factory());
		}

		public ITypeInstanceBuilder<TInstance> CtorArg<TValue>(Func<Context, TValue> factory)
		{
			return CtorArg(typeof(TValue), (ctx, type) => factory(ctx));
		}

		public ITypeInstanceBuilder<T> As<T>()
		{
			return new TypeInstanceBuilder<T>(ServiceDescriptor);
		}
	}
}