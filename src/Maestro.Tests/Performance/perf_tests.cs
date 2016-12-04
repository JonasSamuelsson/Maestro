using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using Xunit;
using Xunit.Abstractions;

namespace Maestro.Tests.Performance
{
	public class perf_tests : IDisposable
	{
		private const int Iterations = 100 * 1000;
		private readonly Dictionary<string, object> _dictionary;
		private readonly ITestOutputHelper _output;

		public perf_tests(ITestOutputHelper output)
		{
			_output = output;

			_dictionary = new Dictionary<string, object>
			{
				{"build config", GetBuildConfig()},
				{"iterations", Iterations},
				{"cpu", new ManagementObject("Win32_Processor.DeviceID='CPU0'")["CurrentClockSpeed"]}
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
			Complex();
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
		public void Complex()
		{
			var container = new Container(x =>
			{
				x.Service<A>().Use.Type<A>();
				x.Service<B>().Use.Type<B>();
				x.Service<C>().Use.Type<C>();
				x.Service<D>().Use.Type<D>();
			});
			Benchmark(() => container.GetService<A>(), "complex");
		}

		[Fact(Skip = "run manually")]
		public void ChildContainer()
		{
			var parentContainer = new Container(x => x.Service<C>().Use.Type<C>());
			var childContainer = parentContainer.GetChildContainer(x => x.Service<D>().Use.Type<D>());
			Benchmark(() => childContainer.GetService<A>(), "child container");
		}

		private void Benchmark(Action action, string info)
		{
			GC.Collect();
			action();
			var stopwatch = Stopwatch.StartNew();
			for (var i = 0; i < Iterations; i++) action();
			var elapsed = stopwatch.Elapsed;
			_dictionary.Add(info, elapsed.TotalMilliseconds.ToString("0") + "ms");
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

		class A { public A(B b, C c, D d) { } }
		class B { public B(C c, D d) { } }
		class C { public C(D d) { } }
		class D { public D() { } }
	}
}