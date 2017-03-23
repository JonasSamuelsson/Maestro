namespace Maestro.Configuration
{
	public interface IUseServiceExpression
	{
		IInstanceKindSelector Use { get; }
	}

	public interface IUseServiceExpression<T>
	{
		IInstanceKindSelector<T> Use { get; }
	}
}