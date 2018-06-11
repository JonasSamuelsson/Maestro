namespace Maestro.Internals
{
	internal abstract class Pipeline
	{
		internal abstract PipelineType PipelineType { get; }

		internal abstract object Execute(Context context);
	}
}