namespace Maestro.Fluent
{
	public interface ITypeInstanceBuilder<TBuilder> : IOnCreateExpression<ITypeInstanceBuilder<TBuilder>>,
		ILifecycleExpression<ITypeInstanceBuilder<TBuilder>>,
		IOnActivateExpression<ITypeInstanceBuilder<TBuilder>>
	{
	}
}