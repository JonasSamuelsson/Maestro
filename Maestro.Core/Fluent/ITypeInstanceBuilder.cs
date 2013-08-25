namespace Maestro.Fluent
{
	public interface ITypeInstanceBuilder<T> : IOnCreateExpression<ITypeInstanceBuilder<T>>,
		ILifecycleExpression<ITypeInstanceBuilder<T>>,
		IOnActivateExpression<ITypeInstanceBuilder<T>>
	{
	}
}