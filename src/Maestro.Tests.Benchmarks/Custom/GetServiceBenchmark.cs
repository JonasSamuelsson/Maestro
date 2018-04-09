using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Maestro.Tests.Benchmarks.Custom
{
	public static class GetServiceBenchmark
	{
		public static void Execute()
		{
			Execute<List<int>>();
		}

		private static void Execute<T>()
		{
			new Container().GetService<T>();

			for (var i = 0; i < 5; i++)
			{
				var stopwatch = new Stopwatch();

				for (var j = 0; j < 100_000; j++)
				{
					var container = new Container();
					stopwatch.Start();
					container.GetService<T>();
					stopwatch.Stop();
				}

				Console.WriteLine($"{stopwatch.Elapsed.TotalMilliseconds} ms");
			}
		}
	}
}