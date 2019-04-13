using BenchmarkDotNet.Running;

namespace Maestro.Benchmarks
{
	static class Program
	{
		public static void Main() => BenchmarkRunner.Run<BenchmarkDotNet>();
	}
}