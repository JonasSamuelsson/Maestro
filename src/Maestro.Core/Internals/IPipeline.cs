namespace Maestro.Internals
{
	interface IPipeline
	{
		PipelineType PipelineType { get; }
		object Execute(Context context);
	}
}