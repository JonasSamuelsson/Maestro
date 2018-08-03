using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Configuration;
using Maestro.Conventions;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Conventions
{
	public class conventional_registration
	{
		[Fact]
		public void should_use_provided_registrator()
		{
			var types = new[] { typeof(object) };
			var convention = new Convention();
			var container = new Container();

			container.Configure(x => x.Scan(_ => _.Types(types).Using(convention)));

			convention.ProcessedTypes.ShouldBe(types);
		}

		[Fact]
		public void should_filter_types_using_provided_lambda()
		{
			var convention = new Convention();
			var container = new Container();

			container.Configure(x => x.Scan(_ => _.Types(new[] { typeof(object), typeof(int) }).Where(t => t.IsClass).Using(convention)));

			convention.ProcessedTypes.ShouldBe(new[] { typeof(object) });
		}

		[Fact]
		public void should_filter_types_using_provided_filter()
		{
			var convention = new Convention();
			var container = new Container();

			container.Configure(x => x.Scan(_ => _.Types(new[] { typeof(object), typeof(int) }).Where(t => t.IsClass).Using(convention)));

			convention.ProcessedTypes.ShouldBe(new[] { typeof(object) });
		}

		[Fact]
		public void type_should_match_all_filters_to_get_processed()
		{
			var convention = new Convention();
			var container = new Container();

			container.Configure(x => x.Scan(_ => _.Types(new[] { typeof(object), typeof(int), typeof(string) }).Where(t => t.IsClass).Where(t => t.Name.Contains("n")).Using(convention)));

			convention.ProcessedTypes.ShouldBe(new[] { typeof(string) });
		}

		private class Convention : IConvention
		{
			public Convention()
			{
				ProcessedTypes = Enumerable.Empty<Type>();
			}

			public IEnumerable<Type> ProcessedTypes { get; private set; }

			public void Process(IEnumerable<Type> types, ContainerBuilder containerBuilder)
			{
				ProcessedTypes = types.ToList();
			}
		}
	}
}