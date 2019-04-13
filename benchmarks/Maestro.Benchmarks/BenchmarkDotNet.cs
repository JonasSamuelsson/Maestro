using BenchmarkDotNet.Attributes;

namespace Maestro.Benchmarks
{
	//[IterationTime(400)]
	//[ClrJob, CoreJob]
	[MemoryDiagnoser, HtmlExporter, PlainExporter]
	public class BenchmarkDotNet
	{
		private static readonly IContainer Container = new Container(x =>
		{
			x.Add<Types.Transient>().Self();
			x.Add<Types.Scoped>().Self().Scoped();
			x.Add<Types.Singleton>().Self().Singleton();
			x.Add(typeof(Types.Generic<>)).Self();
			x.Add<Types.PropertyInjection>().Self().SetProperty(y => y.Transient);
			x.Add<Types.Complex0>().Self();
			x.Add<Types.Complex1>().Self();
			x.Add<Types.Complex2>().Self();
			x.Add<Types.Complex3>().Self();
			x.Add<Types.Enumerable>().Type<Types.Enumerable1>();
			x.Add<Types.Enumerable>().Type<Types.Enumerable2>();
			x.Add<Types.Enumerable>().Type<Types.Enumerable3>();
			x.Add<Types.Enumerable>().Type<Types.Enumerable4>();
			x.Add<Types.Enumerable>().Type<Types.Enumerable5>();
		});

		private static readonly IScope Scope = Container.CreateScope();

		[Benchmark(Baseline = true)]
		public void Baseline() => new Types.Transient();

		[Benchmark]
		public void Transient() => Container.GetService<Types.Transient>();

		[Benchmark]
		public void Scoped() => Container.GetService<Types.Scoped>();

		[Benchmark]
		public void Singleton() => Container.GetService<Types.Singleton>();

		[Benchmark]
		public void TransientFromNewScope()
		{
			using (var container = Container.CreateScope())
				container.GetService<Types.Transient>();
		}

		[Benchmark]
		public void TransientFromReusedScope() => Scope.GetService<Types.Transient>();

		[Benchmark]
		public void ScopedFromNewScope()
		{
			using (var container = Container.CreateScope())
				container.GetService<Types.Scoped>();
		}

		[Benchmark]
		public void ScopedFromReusedScope() => Scope.GetService<Types.Scoped>();

		[Benchmark]
		public void Generic() => Container.GetService<Types.Generic<object>>();

		[Benchmark]
		public void Enumerable_5() => Container.GetServices<Types.Enumerable>();

		[Benchmark]
		public void PropertyInjection_2() => Container.GetService<Types.PropertyInjection>();

		[Benchmark]
		public void Complex_8() => Container.GetService<Types.Complex3>();

		class Types
		{
			internal class Transient { }

			internal class Scoped { }

			internal class Singleton { }

			internal class Generic<T> { }

			internal class PropertyInjection
			{
				internal Transient Transient { get; set; }
			}

			internal class Complex0 { }
			internal class Complex1 { internal Complex1(Complex0 c0) { } }
			internal class Complex2 { internal Complex2(Complex1 c1, Complex0 c0) { } }
			internal class Complex3 { internal Complex3(Complex2 c2, Complex1 c1, Complex0 c0) { } }

			internal abstract class Enumerable { }
			internal class Enumerable1 : Enumerable { }
			internal class Enumerable2 : Enumerable { }
			internal class Enumerable3 : Enumerable { }
			internal class Enumerable4 : Enumerable { }
			internal class Enumerable5 : Enumerable { }
		}
	}
}