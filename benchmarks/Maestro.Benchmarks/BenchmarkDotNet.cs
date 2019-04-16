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
			x.Add<Types.Complex10>().Self();
			x.Add<Types.Complex21>().Self();
			x.Add<Types.Complex31>().Self();
			x.Add<Types.Complex41>().Self();
			x.Add<Types.Complex42>().Self();
			x.Add<Types.Complex51>().Self();
			x.Add<Types.Complex61>().Self();
			x.Add<Types.Complex71>().Self();
			x.Add<Types.Complex81>().Self();
			x.Add<Types.Complex83>().Self();
			x.Add<Types.Complex87>().Self();
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
		public void Complex_8() => Container.GetService<Types.Complex83>();

		[Benchmark]
		public void ComplexDeep_8() => Container.GetService<Types.Complex81>();

		[Benchmark]
		public void ComplexWide_8() => Container.GetService<Types.Complex87>();

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

			internal class Complex10 { }
			internal class Complex21 { internal Complex21(Complex10 c10) { } }
			internal class Complex31 { internal Complex31(Complex21 c21) { } }
			internal class Complex41 { internal Complex41(Complex31 c31) { } }
			internal class Complex42 { internal Complex42(Complex21 c21, Complex10 c10) { } }
			internal class Complex51 { internal Complex51(Complex41 c41) { } }
			internal class Complex61 { internal Complex61(Complex51 c51) { } }
			internal class Complex71 { internal Complex71(Complex61 c61) { } }
			internal class Complex81 { internal Complex81(Complex71 c71) { } }
			internal class Complex83 { internal Complex83(Complex42 c42, Complex21 c21, Complex10 c10) { } }
			internal class Complex87 { internal Complex87(Complex10 c10_1, Complex10 c10_2, Complex10 c10_3, Complex10 c10_4, Complex10 c10_5, Complex10 c10_6, Complex10 c10_7) { } }

			internal abstract class Enumerable { }
			internal class Enumerable1 : Enumerable { }
			internal class Enumerable2 : Enumerable { }
			internal class Enumerable3 : Enumerable { }
			internal class Enumerable4 : Enumerable { }
			internal class Enumerable5 : Enumerable { }
		}
	}
}