namespace Maestro.Fluent
{
	public interface ILambdaInstanceBuilder<TInstance> : IOnCreateExpression<TInstance, ILambdaInstanceBuilder<TInstance>>,
		ILifetimeExpression<ILambdaInstanceBuilder<TInstance>>,
		IInterceptExpression<TInstance, ILambdaInstanceBuilder<TInstance>>
	{
	}
}