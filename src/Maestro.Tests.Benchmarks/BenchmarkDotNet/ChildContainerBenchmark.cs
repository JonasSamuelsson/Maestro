using BenchmarkDotNet.Attributes;

namespace Maestro.Tests.Benchmarks.BenchmarkDotNet
{
	[MemoryDiagnoser]
	public class ChildContainerBenchmark
	{
		static readonly Container Container = new Container(x => x.Use<object>().Self());

		[Benchmark]
		public void Execute()
		{
			using (var container = Container.GetChildContainer())
				container.GetService<object>();
		}
	}
}