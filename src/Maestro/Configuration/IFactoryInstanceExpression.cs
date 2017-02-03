namespace Maestro.Configuration
{
	public interface IFactoryInstanceExpression<T> : IInstanceExpression<T, IFactoryInstanceExpression<T>>
	{ }
}