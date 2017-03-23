namespace Maestro.Configuration
{
	public interface IServiceExpression : IUseServiceExpression, ITryUseServiceExpression, IAddServiceExpression
	{
	}

	public interface IServiceExpression<T> : IUseServiceExpression<T>, ITryUseServiceExpression<T>, IAddServiceExpression<T>
	{
	}
}