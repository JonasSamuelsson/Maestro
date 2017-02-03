namespace Maestro.Configuration
{
	public interface INamedServiceExpression : IUseServiceExpression, ITryUseServiceExpression
	{
	}

	public interface INamedServiceExpression<T> : IUseServiceExpression<T>, ITryUseServiceExpression<T>
	{
	}
}