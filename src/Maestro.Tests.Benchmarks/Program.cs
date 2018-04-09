using Maestro.Tests.Benchmarks.Custom;

namespace Maestro.Tests.Benchmarks
{
	class Program
	{
		//public static void Main() => BenchmarkRunner.Run<ChildContainerBenchmark>();

		//public static void Main() => BenchmarkRunner.Run<DefaultBenchmarks>();

		public static void Main() => MemoryConsumptionBenchmark.Execute();
	}
}
