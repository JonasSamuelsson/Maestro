using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace Maestro.Tests.Core.Performance
{
	public class performance_tests
	{
		const int Iterations = 100 * 1000;

		[Fact]
		public void new_container()
		{
			Warmup(Baseline);
			Warmup(() => GetConfiguredContainer().GetService<C3>());

			Console.WriteLine(Measure(() => GetConfiguredContainer().GetService<C3>()) / Measure(Baseline));
		}

		[Fact]
		public void child_container()
		{
			Warmup(Baseline);
			Warmup(() => GetConfiguredContainer().GetChildContainer().GetService<C3>());

			var container = GetConfiguredContainer();
			Console.WriteLine(Measure(() => container.GetChildContainer().GetService<C3>()) / Measure(Baseline));
		}

		[Fact]
		public void reuse_container()
		{
			Warmup(Baseline);
			Warmup(() => GetConfiguredContainer().GetService<C3>());

			var container = GetConfiguredContainer();
			Console.WriteLine(Measure(() => container.GetService<C3>()) / Measure(Baseline));
		}

		[Fact]
		public void property_injection()
		{
			var container = new Container(x => x.Use<P>().Type<P>().SetProperty("O", new object()));
			Action baseline = () => new P { O = new object() };
			Action work = () => container.GetService<P>();
			Compare(baseline, work);
		}

		[Fact]
		public void get_generics()
		{
			var container = new Container(x => x.Use(typeof(IList<>)).Type(typeof(List<>)));
			Action baseline = () => new List<string>();
			Action work = () => container.GetService<IList<string>>();
			Compare(baseline, work);
		}

		[Fact]
		public void get_all_generics()
		{
			var container = new Container(x =>
													{
														x.Add(typeof(IList<>)).Instance(typeof(List<>));
														x.Add(typeof(IList<>)).Instance(typeof(List<>));
													});
			Action baseline = delegate { var temp = new[] { new List<string>(), new List<string>() }; };
			Action work = () => container.GetServices<IList<string>>();
			Compare(baseline, work);
		}

		static void Compare(Action baseline, Action work)
		{
			Warmup(baseline);
			Warmup(work);

			Console.WriteLine(Measure(work) / Measure(baseline));
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
				x.Use<C0>().Type<C0>();
				x.Use<C1>().Type<C1>();
				x.Use<C2>().Type<C2>();
				x.Use<C3>().Type<C3>();
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