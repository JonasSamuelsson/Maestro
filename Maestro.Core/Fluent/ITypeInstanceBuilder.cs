namespace Maestro.Fluent
{
	public interface ITypeInstanceBuilder<TInstance> : ILifetimeExpression<ITypeInstanceBuilder<TInstance>>,
		IInterceptExpression<TInstance, ITypeInstanceBuilder<TInstance>>
	{
	}
}