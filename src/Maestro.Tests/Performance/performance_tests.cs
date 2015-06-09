using System;
using System.Diagnostics;
using Xunit;

namespace Maestro.Tests.Performance
{
	public class performance_tests
	{
		const int Iterations = 10 * 1000;

		[Fact]
		public void reuse_container()
		{
			var container = GetConfiguredContainer();
			Console.WriteLine(Measure(() => container.Get<C3>()) / Measure(Baseline));
		}

		[Fact]
		public void child_container()
		{
			var container = GetConfiguredContainer();
			Console.WriteLine(Measure(() => container.GetChildContainer().Get<C3>()) / Measure(Baseline));
		}

		[Fact]
		public void new_container()
		{
			Console.WriteLine(Measure(() => GetConfiguredContainer().Get<C3>()) / Measure(Baseline));
		}

		private static void Baseline()
		{
			new C3(new C0(), new C1(new C0()), new C2(new C0(), new C1(new C0())));
		}

		private static double Measure(Action action)
		{
			var stopwatch = Stopwatch.StartNew();
			for (int i = 0; i < Iterations; i++) action();
			return stopwatch.Elapsed.TotalMilliseconds;
		}

		private static Container GetConfiguredContainer()
		{
			var container = new Container(x =>
			{
				x.For<C0>().Use<C0>();
				x.For<C1>().Use<C1>();
				x.For<C2>().Use<C2>();
				x.For<C3>().Use<C3>();
			});
			return container;
		}

		class C0 { }
		class C1 { public C1(C0 c0) { } }
		class C2 { public C2(C0 c0, C1 c1) { } }
		class C3 { public C3(C0 c0, C1 c1, C2 c2) { } }
	}
}