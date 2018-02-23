using BenchmarkDotNet.Running;

namespace Maestro.Benchmarks
{
	class Program
	{
		public static void Main(string[] args) => BenchmarkRunner.Run<Tests>();
	}
}
