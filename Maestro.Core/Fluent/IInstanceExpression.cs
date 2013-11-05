using System;

namespace Maestro.Fluent
{
	public interface IInstanceExpression<TPlugin>
	{
		void Use<TInstance>(TInstance instance) where TInstance : TPlugin;
		IInstanceBuilder<TInstance> Use<TInstance>(Func<TInstance> lambda) where TInstance : TPlugin;
		IInstanceBuilder<TInstance> Use<TInstance>(Func<IContext, TInstance> lambda) where TInstance : TPlugin;
		IInstanceBuilder<TInstance> Use<TInstance>() where TInstance : TPlugin;
		IInstanceBuilder<TPlugin> Use(Type type);
		void UseConditional(Action<IConditionalInstanceBuilder<TPlugin>> action);
	}
}