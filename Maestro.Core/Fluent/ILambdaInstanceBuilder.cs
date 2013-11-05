namespace Maestro.Fluent
{
	public interface ILambdaInstanceBuilder<TInstance> : ILifetimeExpression<ILambdaInstanceBuilder<TInstance>>,
		IInterceptExpression<TInstance, ILambdaInstanceBuilder<TInstance>>
	{
	}
}