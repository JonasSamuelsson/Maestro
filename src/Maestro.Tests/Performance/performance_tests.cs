using System;
using System.Diagnostics;
using Xunit;

namespace Maestro.Tests.Performance
{
	public class performance_tests
	{
		const int Iterations = 100 * 1000;

		[Fact(Skip = "performance")]
		public void new_container()
		{
			Warmup(Baseline);
			Warmup(() => GetConfiguredContainer().Get<C3>());

			Console.WriteLine(Measure(() => GetConfiguredContainer().Get<C3>()) / Measure(Baseline));
		}

		[Fact(Skip = "performance")]
		public void child_container()
		{
			Warmup(Baseline);
			Warmup(() => GetConfiguredContainer().GetChildContainer().Get<C3>());

			var container = GetConfiguredContainer();
			Console.WriteLine(Measure(() => container.GetChildContainer().Get<C3>()) / Measure(Baseline));
		}

		[Fact(Skip = "performance")]
		public void reuse_container()
		{
			Warmup(Baseline);
			Warmup(() => GetConfiguredContainer().Get<C3>());

			var container = GetConfiguredContainer();
			Console.WriteLine(Measure(() => container.Get<C3>()) / Measure(Baseline));
		}

		[Fact(Skip = "performance")]
		public void property_injection()
		{
			Action baseline = () => new P { O = new object() };
			var container = new Container(x => x.For<P>().Use<P>().SetProperty(y => y.O));
			Action work = () => container.Get<P>();

			Warmup(baseline);
			Warmup(work);

			Console.WriteLine(Measure(() => work()) / Measure(() => baseline()));
		}

		private static void Baseline()
		{
			new C3(new C2(new C1(new C0()), new C0()), new C1(new C0()), new C0());
		}

		private static void Warmup(Action action)
		{
			action();
		}

		private static double Measure(Action action)
		{
			var stopwatch = Stopwatch.StartNew();
			for (var i = 0; i < Iterations; i++) action();
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
		class C2 { public C2(C1 c1, C0 c0) { } }
		class C3 { public C3(C2 c2, C1 c1, C0 c0) { } }
		class P { public object O { get; set; } }
	}
}