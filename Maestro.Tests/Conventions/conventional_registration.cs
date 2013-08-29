using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Maestro.Conventions;
using Xunit;

namespace Maestro.Tests.Conventions
{
	public class conventional_registration
	{
		[Fact]
		public void should_use_provided_registrator()
		{
			var types = new[] { typeof(object) };
			var registrator = new Convention();
			var container = new Container();

			container.Configure(x => x.Scan.Types(types).Using(registrator));

			registrator.ProcessedTypes.Should().BeEquivalentTo(types);
		}

		[Fact]
		public void should_filter_types_using_provided_lambda()
		{
			var registrator = new Convention();
			var container = new Container();

			container.Configure(x => x.Scan.Types(new[] { typeof(object), typeof(int) }).Where(t => t.IsClass).Using(registrator));

			registrator.ProcessedTypes.Should().BeEquivalentTo(new[] { typeof(object) });
		}

		[Fact]
		public void should_filter_types_using_provided_filter()
		{
			var registrator = new Convention();
			var container = new Container();

			container.Configure(x => x.Scan.Types(new[] { typeof(object), typeof(int) }).Matching(new IsClassFilter()).Using(registrator));

			registrator.ProcessedTypes.Should().BeEquivalentTo(new[] { typeof(object) });
		}

		[Fact]
		public void type_should_match_all_filters_to_get_processed()
		{
			var registrator = new Convention();
			var container = new Container();

			container.Configure(x => x.Scan
				.Types(new[] { typeof(object), typeof(int), typeof(string) })
				.Where(t => t.IsClass)
				.Where(t => t.Name.Contains("n"))
				.Using(registrator));

			registrator.ProcessedTypes.Should().BeEquivalentTo(new[] { typeof(string) });
		}

		private class Convention : IConvention
		{
			public Convention()
			{
				ProcessedTypes = Enumerable.Empty<Type>();
			}

			public IEnumerable<Type> ProcessedTypes { get; private set; }

			public void Process(IEnumerable<Type> types, IContainerConfiguration containerConfiguration)
			{
				ProcessedTypes = types.ToList();
			}
		}

		public class IsClassFilter : IConventionFilter
		{
			public bool IsMatch(Type type)
			{
				return type.IsClass;
			}
		}
	}
}