namespace Maestro.Fluent
{
	public interface IOnCreateExpression<TInstance, TParent>
	{
		IInterceptExpression<TInstance, TParent> OnCreate { get; }
	}
}