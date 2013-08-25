namespace Maestro.Fluent
{
	public interface IOnCreateExpression<TParent>
	{
		IInterceptExpression<TParent> OnCreate { get; }
	}
}