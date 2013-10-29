namespace Maestro.Fluent
{
	public interface ITypeInstanceBuilder<TInstance> : IOnCreateExpression<TInstance, ITypeInstanceBuilder<TInstance>>,
		ILifetimeExpression<ITypeInstanceBuilder<TInstance>>,
		IInterceptExpression<TInstance, ITypeInstanceBuilder<TInstance>>
	{
	}
}