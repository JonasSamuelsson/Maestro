using Maestro.Tests.Performance;

namespace Maestro.Tests
{
	static class Program
	{
		static void Main()
		{
			new perf_tests().MultiThreaded();
		}
	}
}
