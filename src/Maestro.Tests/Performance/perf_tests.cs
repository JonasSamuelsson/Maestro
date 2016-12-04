using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Maestro.Tests.Performance
{
	public class perf_tests : IDisposable
	{
		private const int Iterations = 1000 * 1000;
		private readonly Dictionary<string, object> _dictionary;
		private readonly ITestOutputHelper _output;

		public perf_tests(ITestOutputHelper output)
		{
			_output = output;

			_dictionary = new Dictionary<string, object>
			{
				{"build config", GetBuildConfig()},
				{"iterations", Iterations},
				{"cpu count", Environment.ProcessorCount},
				{"cpu speed", new ManagementObject("Win32_Processor.DeviceID='CPU0'")["CurrentClockSpeed"]}
			};
		}

		[Fact(Skip = "run manually")]
		public void Full()
		{
			Instance();
			Factory();
			Type();
			Singleton();
			Enumerable();
			CtorInjection();
			PropertyInjection();
			Complex();
			MultiThreaded();
			ChildContainer();
		}

		[Fact(Skip = "run manually")]
		public void Instance()
		{
			var container = new Container(x => x.Service<object>().Use.Instance(new object()));
			Benchmark(() => container.GetService<object>(), "instance");
		}

		[Fact(Skip = "run manually")]
		public void Factory()
		{
			var container = new Container(x => x.Service<object>().Use.Factory(() => new object()));
			Benchmark(() => container.GetService<object>(), "factory");
		}

		[Fact(Skip = "run manually")]
		public void Type()
		{
			var container = new Container(x => x.Service<object>().Use.Type<object>());
			Benchmark(() => container.GetService<object>(), "type");
		}

		[Fact(Skip = "run manually")]
		public void Singleton()
		{
			var container = new Container(x => x.Service<object>().Use.Type<object>().Lifetime.Singleton());
			Benchmark(() => container.GetService<object>(), "singleton");
		}

		[Fact(Skip = "run manually")]
		public void Enumerable()
		{
			var container = new Container(x =>
			{
				x.Services<object>().Add.Type<object>();
				x.Services<object>().Add.Type<object>();
				x.Services<object>().Add.Type<object>();
				x.Services<object>().Add.Type<object>();
				x.Services<object>().Add.Type<object>();
			});
			Benchmark(() => container.GetServices<object>(), "enumerable");
		}

		[Fact(Skip = "run manually")]
		public void CtorInjection()
		{
			var container = new Container(x =>
			{
				x.Service<CtorDependency>().Use.Type<CtorDependency>();
				x.Service<object>().Use.Type<object>();
			});
			Benchmark(() => container.GetService<CtorDependency>(), "ctor injection");
		}

		[Fact(Skip = "run manually")]
		public void PropertyInjection()
		{
			var container = new Container(x =>
			{
				x.Service<PropertyDependency>().Use.Type<PropertyDependency>().SetProperty(y => y.O);
				x.Service<object>().Use.Type<object>();
			});
			Benchmark(() => container.GetService<CtorDependency>(), "property injection");
		}

		[Fact(Skip = "run manually")]
		public void Complex()
		{
			var container = new Container(x =>
			{
				x.Service<Complex1>().Use.Type<Complex1>();
				x.Service<Complex2>().Use.Type<Complex2>();
				x.Service<Complex3>().Use.Type<Complex3>();
				x.Service<Complex4>().Use.Type<Complex4>();
			});
			Benchmark(() => container.GetService<Complex1>(), "complex");
		}

		[Fact(Skip = "run manually")]
		public void MultiThreaded()
		{
			var container = new Container(x =>
			{
				x.Service<Complex1>().Use.Type<Complex1>();
				x.Service<Complex2>().Use.Type<Complex2>();
				x.Service<Complex3>().Use.Type<Complex3>();
				x.Service<Complex4>().Use.Type<Complex4>();
			});
			Benchmark(() => container.GetService<Complex1>(), "multi threaded");
		}

		[Fact(Skip = "run manually")]
		public void ChildContainer()
		{
			var parentContainer = new Container(x => x.Service<CtorDependency>().Use.Type<CtorDependency>());
			var childContainer = parentContainer.GetChildContainer(x => x.Service<object>().Use.Type<object>());
			Benchmark(() => childContainer.GetService<CtorDependency>(), "child container");
		}

		private void Benchmark(Action action, string info, bool multiThreaded = false)
		{
			Execute(action, 1);
			GC.Collect();

			var stopwatch = Stopwatch.StartNew();
			if (multiThreaded == false)
			{
				Execute(action, Iterations);
			}
			else
			{
				var tasks = System.Linq.Enumerable.Range(0, Environment.ProcessorCount)
					.Select(_ => Task.Factory.StartNew(() => Execute(action, Iterations)))
					.ToList();
				Task.WhenAll(tasks).Wait();
			}
			var elapsed = stopwatch.Elapsed;
			_dictionary.Add(info, elapsed.TotalMilliseconds.ToString("0") + "ms");
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
				_output.WriteLine($"{timing.Key.PadRight(keyLength)} : {timing.Value}");
			}
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