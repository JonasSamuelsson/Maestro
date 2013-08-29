namespace Maestro.Fluent
{
	public interface ILambdaInstanceBuilder<TInstance> : IOnCreateExpression<TInstance, ILambdaInstanceBuilder<TInstance>>,
		ILifetimeExpression<ILambdaInstanceBuilder<TInstance>>,
		IOnActivateExpression<TInstance, ILambdaInstanceBuilder<TInstance>>
	{
	}
}