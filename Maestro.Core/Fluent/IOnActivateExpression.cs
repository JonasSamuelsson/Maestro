namespace Maestro.Fluent
{
	public interface IOnActivateExpression<TParent>
	{
		IInterceptExpression<TParent> OnActivate { get; }
	}
}