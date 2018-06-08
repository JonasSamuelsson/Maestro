using System;
using System.Diagnostics;

namespace Maestro.Tests.Benchmarks.Custom
{
	public static class GetServiceBenchmark
	{
		public static void Execute()
		{
			Execute<EventArgs>();
		}

		private static void Execute<T>()
		{
			CreateContainer().GetService<T>();
			Test<T>();
		}

		private static Container CreateContainer()
		{
			return new Container(x => x.TypeProviders.Add(new MyTypeProvider()));
		}

		private static void Test<T>()
		{
			for (var i = 0; i < 5; i++)
			{
				var stopwatch = new Stopwatch();

				for (var j = 0; j < 100_000; j++)
				{
					var container = CreateContainer();
					stopwatch.Start();
					container.GetService<T>();
					stopwatch.Stop();
				}

				Console.WriteLine($"{stopwatch.Elapsed.TotalMilliseconds} ms");
			}
		}

		private class MyTypeProvider : ITypeProvider
		{
			public Type GetInstanceTypeOrNull(Type serviceType, Context context)
			{
				return serviceType == typeof(object) ? typeof(EventArgs) : null;
			}
		}
	}
}