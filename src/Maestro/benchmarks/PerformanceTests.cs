﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Maestro.Benchmarks
{
	public class PerformanceTests : IDisposable
	{
		private const int Iterations = 50 * 1000;
		private readonly Dictionary<string, object> _dictionary;
		private readonly ITestOutputHelper _output;

		public PerformanceTests(ITestOutputHelper output = null)
		{
			_output = output;

			_dictionary = new Dictionary<string, object>
			{
				{"build config", GetBuildConfig()},
				{"iterations", Iterations},
				{"cpu count", Environment.ProcessorCount},
				//{"cpu speed", new ManagementObject("Win32_Processor.DeviceID='CPU0'")["CurrentClockSpeed"]}
			};
		}

		[Fact]
		public void Full()
		{
			BaselineSimple();
			BaselineComplex();
			BaselineEnumerable();
			Instance();
			Factory();
			Type();
			Singleton();
			Enumerable();
			CtorInjection();
			PropertyInjection();
			Complex();
			MultiThreaded();
			ScopedContainer();
		}

		[Fact]
		public void BaselineSimple()
		{
			Action action = () => new object();
			Benchmark(action, "baseline simple", 1);
		}

		[Fact]
		public void BaselineComplex()
		{
			Action action = () => new Complex1(
				new Complex2(
					new Complex3(
						new Complex4()),
					new Complex4()),
				new Complex3(
					new Complex4()),
				new Complex4());
			Benchmark(action, "baseline complex", 8);
		}

		[Fact]
		public void BaselineEnumerable()
		{
			Action action = () => new List<object>(new[] { new object(), new object(), new object(), new object(), new object() });
			Benchmark(action, "baseline enumerable", 5);
		}

		[Fact]
		public void Instance()
		{
			var container = new Container(x => x.Add<object>().Instance(new object()));
			Benchmark(() => container.GetService<object>(), "instance", 1);
		}

		[Fact]
		public void Factory()
		{
			var container = new Container(x => x.Add<object>().Factory(() => new object()));
			Benchmark(() => container.GetService<object>(), "factory", 1);
		}

		[Fact]
		public void Type()
		{
			var container = new Container(x => x.Add<object>().Type<object>());
			Benchmark(() => container.GetService<object>(), "type", 1);
		}

		[Fact]
		public void Singleton()
		{
			var container = new Container(x => x.Add<object>().Type<object>().Singleton());
			Benchmark(() => container.GetService<object>(), "singleton", 1);
		}

		[Fact]
		public void Enumerable()
		{
			var container = new Container(x =>
			{
				x.Add<object>().Type<object>();
				x.Add<object>().Type<object>();
				x.Add<object>().Type<object>();
				x.Add<object>().Type<object>();
				x.Add<object>().Type<object>();
			});
			Benchmark(() => container.GetServices<object>(), "enumerable", 5);
		}

		[Fact]
		public void CtorInjection()
		{
			var container = new Container(x =>
			{
				x.Add<CtorDependency>().Type<CtorDependency>();
				x.Add<object>().Type<object>();
			});
			Benchmark(() => container.GetService<CtorDependency>(), "ctor injection", 2);
		}

		[Fact]
		public void PropertyInjection()
		{
			var container = new Container(x =>
			{
				x.Add<PropertyDependency>().Type<PropertyDependency>().SetProperty(y => y.O);
				x.Add<object>().Type<object>();
			});
			Benchmark(() => container.GetService<PropertyDependency>(), "property injection", 2);
		}

		[Fact]
		public void Complex()
		{
			var container = new Container(x =>
			{
				x.Add<Complex1>().Type<Complex1>();
				x.Add<Complex2>().Type<Complex2>();
				x.Add<Complex3>().Type<Complex3>();
				x.Add<Complex4>().Type<Complex4>();
			});
			Benchmark(() => container.GetService<Complex1>(), "complex", 8);
		}

		[Fact]
		public void MultiThreaded()
		{
			var container = new Container(x =>
			{
				x.Add<Complex1>().Type<Complex1>();
				x.Add<Complex2>().Type<Complex2>();
				x.Add<Complex3>().Type<Complex3>();
				x.Add<Complex4>().Type<Complex4>();
			});
			var concurrentWorkers = Environment.ProcessorCount;
			var instances = 8 * concurrentWorkers;
			Benchmark(() => container.GetService<Complex1>(), $"complex ({concurrentWorkers} workers)", instances, concurrentWorkers);
		}

		[Fact]
		public void ScopedContainer()
		{
			var container = new Container(x =>
			{
				x.Add<object>().Type<object>();
				x.Add<CtorDependency>().Type<CtorDependency>();
			});
			var scopedContainer = container.CreateScope();
			Benchmark(() => scopedContainer.GetService<CtorDependency>(), "child container", 2);
		}

		private void Benchmark(Action action, string info, int instances, int concurrentWorkers = 1)
		{
			Execute(action, 1);
			GC.Collect();
			GC.WaitForPendingFinalizers();

			var stopwatch = Stopwatch.StartNew();
			ExecuteTest(action, concurrentWorkers);
			var elapsed = stopwatch.Elapsed;
			_dictionary.Add(info, $"{elapsed.TotalMilliseconds:0} ms, perf-index {100 * elapsed.Ticks / (instances * Iterations):0}");
		}

		private static void ExecuteTest(Action action, int concurrentWorkers)
		{
			if (concurrentWorkers <= 1)
			{
				Execute(action, Iterations);
			}
			else
			{
				var tasks = System.Linq.Enumerable.Range(0, concurrentWorkers)
					.Select(_ => Task.Factory.StartNew(() => Execute(action, Iterations), TaskCreationOptions.LongRunning))
					.ToList();
				Task.WhenAll(tasks).Wait();
			}
		}

		private static void Execute(Action action, int iterations)
		{
			for (var i = 0; i < iterations; i++) action();
		}

		public void Dispose()
		{
			var keyLength = _dictionary.Keys.Max(x => x.Length);
			foreach (var timing in _dictionary)
			{
				_output?.WriteLine($"{timing.Key.PadRight(keyLength)} : {timing.Value}");
			}
			_output.WriteLine("* perf index, lower is better");
		}

		private static string GetBuildConfig()
		{
#if DEBUG
			return "debug";
#else
			return "release";
#endif
		}

		class CtorDependency { public CtorDependency(object o) { } }
		class PropertyDependency { public object O { get; set; } }
		class Complex1 { public Complex1(Complex2 complex2, Complex3 complex3, Complex4 complex4) { } }
		class Complex2 { public Complex2(Complex3 complex3, Complex4 complex4) { } }
		class Complex3 { public Complex3(Complex4 complex4) { } }
		class Complex4 { public Complex4() { } }
	}
}