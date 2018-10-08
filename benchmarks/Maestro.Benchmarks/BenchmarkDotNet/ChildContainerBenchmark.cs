using BenchmarkDotNet.Attributes;

namespace Maestro.Benchmarks.BenchmarkDotNet
{
	[MemoryDiagnoser]
	public class ChildContainerBenchmark
	{
		static readonly Container Container = new Container(x => x.Add<object>().Self());

		[Benchmark]
		public void Execute()
		{
			using (var container = Container.CreateScope())
				container.GetService<object>();
		}
	}
}