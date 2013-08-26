namespace Maestro.Fluent
{
	public interface ITypeInstanceBuilder<TInstance> : IOnCreateExpression<TInstance, ITypeInstanceBuilder<TInstance>>,
		ILifecycleExpression<ITypeInstanceBuilder<TInstance>>,
		IOnActivateExpression<TInstance, ITypeInstanceBuilder<TInstance>>
	{
	}
}