namespace Maestro
{
	public interface IPipelineItem<T>
	{
		T Clone();
	}
}