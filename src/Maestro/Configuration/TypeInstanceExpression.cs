using System;
using Maestro.FactoryProviders;
using Maestro.Internals;

namespace Maestro.Configuration
{
	class TypeInstanceExpression<TInstance> : InstanceExpression<TInstance, ITypeInstanceExpression<TInstance>>, ITypeInstanceExpression<TInstance>
	{
		public TypeInstanceExpression(ServiceDescriptor serviceDescriptor)
			: base(serviceDescriptor)
		{
		}

		internal override ITypeInstanceExpression<TInstance> Parent => this;

		public ITypeInstanceExpression<TInstance> CtorArg(string argName, object value)
		{
			return CtorArg(argName, (ctx, type) => value);
		}

		public ITypeInstanceExpression<TInstance> CtorArg(string argName, Func<object> factory)
		{
			return CtorArg(argName, (ctx, type) => factory());
		}

		public ITypeInstanceExpression<TInstance> CtorArg(string argName, Func<IContext, object> factory)
		{
			return CtorArg(argName, (ctx, type) => factory(ctx));
		}

		public ITypeInstanceExpression<TInstance> CtorArg(string argName, Func<IContext, Type, object> factory)
		{
			var typeFactoryProvider = (TypeFactoryProvider)ServiceDescriptor.FactoryProvider;
			var ctorArg = new TypeFactoryProvider.CtorArg { Name = argName, Factory = factory };
			typeFactoryProvider.CtorArgs.Add(ctorArg);
			return this;
		}

		public ITypeInstanceExpression<TInstance> CtorArg(Type argType, object value)
		{
			return CtorArg(argType, (ctx, type) => value);
		}

		public ITypeInstanceExpression<TInstance> CtorArg(Type argType, Func<object> factory)
		{
			return CtorArg(argType, (ctx, type) => factory());
		}

		public ITypeInstanceExpression<TInstance> CtorArg(Type argType, Func<IContext, object> factory)
		{
			return CtorArg(argType, (ctx, type) => factory(ctx));
		}

		private ITypeInstanceExpression<TInstance> CtorArg(Type argType, Func<IContext, Type, object> factory)
		{
			var typeFactoryProvider = (TypeFactoryProvider)ServiceDescriptor.FactoryProvider;
			var ctorArg = new TypeFactoryProvider.CtorArg { Type = argType, Factory = factory };
			typeFactoryProvider.CtorArgs.Add(ctorArg);
			return Parent;
		}

		public ITypeInstanceExpression<TInstance> CtorArg<TValue>(TValue value)
		{
			return CtorArg(typeof(TValue), (ctx, type) => value);
		}

		public ITypeInstanceExpression<TInstance> CtorArg<TValue>(Func<TValue> factory)
		{
			return CtorArg(typeof(TValue), (ctx, type) => factory());
		}

		public ITypeInstanceExpression<TInstance> CtorArg<TValue>(Func<IContext, TValue> factory)
		{
			return CtorArg(typeof(TValue), (ctx, type) => factory(ctx));
		}

		public ITypeInstanceExpression<T> As<T>()
		{
			return new TypeInstanceExpression<T>(ServiceDescriptor);
		}
	}
}