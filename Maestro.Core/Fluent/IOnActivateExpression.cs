namespace Maestro.Fluent
{
	public interface IOnActivateExpression<TInstance, TParent>
	{
		IInterceptExpression<TInstance, TParent> OnActivate { get; }
	}
}