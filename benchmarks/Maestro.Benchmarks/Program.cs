using BenchmarkDotNet.Running;
using Maestro.Benchmarks.BenchmarkDotNet;

namespace Maestro.Benchmarks
{
	static class Program
	{
		//public static void Main() => BenchmarkRunner.Run<ChildContainerBenchmark>();

		public static void Main() => BenchmarkRunner.Run<DefaultBenchmarks>();

		//public static void Main() => GetServiceBenchmark.Execute();
	}
}