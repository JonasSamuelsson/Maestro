using Maestro.Fluent;

namespace Maestro
{
	public static class LifetimeExpressionExtensions
	{
		public static T AsSingleton<T>(this T expression) where T : ILifetimeExpression<T>
		{
			return expression.Lifetime.Singleton();
		}
	
		public static T AsTransient<T>(this T expression) where T : ILifetimeExpression<T>
		{
			return expression.Lifetime.Transient();
		}
	}
}