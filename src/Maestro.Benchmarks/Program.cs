using System;
using System.Diagnostics;
using System.Linq;
using BenchmarkDotNet.Running;

namespace Maestro.Benchmarks
{
	class Program
	{
		public static void Main(string[] args) => BenchmarkRunner.Run<Benchmarks>();

		//public static void Main()
		//{
		//	var container = new Container(x => x.Use<object>().Self());

		//	var stopwatch = Stopwatch.StartNew();
		//	for (var i = 1; i <= 1_000_000; i++)
		//	{
		//		using (var childContainer = container.GetChildContainer())
		//			childContainer.GetService<object>();

		//		if (i % 100_000 == 0)
		//		{
		//			stopwatch.Stop();
		//			var duration = stopwatch.Elapsed.TotalMilliseconds;
		//			Console.WriteLine($"{i} {duration}ms");
		//			PrintMemoryStats();
		//			Console.WriteLine();
		//			stopwatch.Start();
		//		}
		//	}
		//}

		//private static void PrintMemoryStats()
		//{
		//	var process = Process.GetCurrentProcess();

		//	var properties = typeof(Process).GetProperties()
		//		.Where(x => x.Name.Contains("64"));

		//	foreach (var property in properties)
		//		Console.WriteLine($"  {Format(property.Name)} {Format((long)property.GetValue(process, null))}");
		//}

		//private static string Format(string name)
		//{
		//	return name.Replace("64", "");
		//}

		//private static string Format(long value)
		//{
		//	var units = new[] { "b", "kB", "MB", "GB", "TB" };
		//	var index = 0;
		//	var d = (decimal)value;
		//	while (d > 1024)
		//	{
		//		index++;
		//		d /= 1024;
		//	}

		//	return $"{d:#.00}{units[index]}";
		//}
	}
}
