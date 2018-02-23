namespace Maestro.Benchmarks
{
	class Program
	{
		//public static void Main() => BenchmarkRunner.Run<ChildContainerBenchmark>();

		//public static void Main() => BenchmarkRunner.Run<DefaultBenchmarks>();

		public static void Main() => ManualBenchmark.Execute();
	}
}
