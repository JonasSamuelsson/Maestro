using Maestro.Tests.Benchmarks.Custom;

namespace Maestro.Tests.Benchmarks
{
	static class Program
	{
		//public static void Main() => BenchmarkRunner.Run<ChildContainerBenchmark>();

		//public static void Main() => BenchmarkRunner.Run<DefaultBenchmarks>();

		public static void Main() => GetServiceBenchmark.Execute();
	}
}