using System;

namespace Maestro.Fluent
{
	public interface IInstanceFactoryExpression<TPlugin>
	{
		void Use<TInstance>(TInstance instance) where TInstance : TPlugin;
		IInstanceBuilderExpression<TInstance> Use<TInstance>(Func<TInstance> lambda) where TInstance : TPlugin;
		IInstanceBuilderExpression<TInstance> Use<TInstance>(Func<IContext, TInstance> lambda) where TInstance : TPlugin;
		IInstanceBuilderExpression<TInstance> Use<TInstance>() where TInstance : TPlugin;
		IInstanceBuilderExpression<TPlugin> Use(Type type);
		void UseConditional(Action<IConditionalInstanceBuilderExpression<TPlugin>> action);
	}
}