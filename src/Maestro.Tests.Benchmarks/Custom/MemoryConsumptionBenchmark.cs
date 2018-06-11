using System;
using System.Diagnostics;
using System.Linq;

namespace Maestro.Tests.Benchmarks.Custom
{
	public class MemoryConsumptionBenchmark
	{
		public static void Execute()
		{
			var container = new Container(x => x.Use<object>().Self());

			Warmup(container);

			var stopwatch = Stopwatch.StartNew();
			Test(container);
			stopwatch.Stop();

			var duration = stopwatch.Elapsed.TotalMilliseconds;
			Console.WriteLine($"{duration}ms");
			PrintMemoryStats();
			Console.WriteLine();
		}

		private static void Warmup(IContainer container)
		{
			DoWork(container);
		}

		private static void Test(IContainer container)
		{
			for (var i = 1; i <= 100_000; i++)
				DoWork(container);
		}

		private static void DoWork(IContainer container)
		{
			using (var childContainer = container.GetScopedContainer())
				childContainer.GetService<object>();
		}

		private static void PrintMemoryStats()
		{
			var process = Process.GetCurrentProcess();

			var properties = typeof(Process).GetProperties()
				.Where(x => x.Name.Contains("64"));

			foreach (var property in properties)
				Console.WriteLine($"  {Format(property.Name)} {Format((long)property.GetValue(process, null))}");
		}

		private static string Format(string name)
		{
			return name.Replace("64", "");
		}

		private static string Format(long value)
		{
			var units = new[] { "b", "kB", "MB", "GB", "TB" };
			var index = 0;
			var d = (decimal)value;
			while (d > 1024)
			{
				index++;
				d /= 1024;
			}

			return $"{d:#.00}{units[index]}";
		}
	}
}