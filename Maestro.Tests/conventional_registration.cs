using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Maestro.Tests
{
	public class conventional_registration
	{
		[Fact]
		public void should_use_provided_registrator()
		{
			var types = new[] { typeof(object) };
			var registrator = new ConventionalRegistrator();
			var container = new Container();

			container.Configure(x => x.Scan.Types(types).Using(registrator));

			registrator.ProcessedTypes.Should().BeEquivalentTo(types);
		}

		[Fact]
		public void should_filter_types_using_provided_lambda()
		{
			var registrator = new ConventionalRegistrator();
			var container = new Container();

			container.Configure(x => x.Scan.Types(new[] { typeof(object), typeof(int) }).Matching(t => t.IsClass).Using(registrator));

			registrator.ProcessedTypes.Should().BeEquivalentTo(new[] { typeof(object) });
		}

		[Fact]
		public void should_filter_types_using_provided_filter()
		{
			var registrator = new ConventionalRegistrator();
			var container = new Container();

			container.Configure(x => x.Scan.Types(new[] { typeof(object), typeof(int) }).Matching(new ClassFilter()).Using(registrator));

			registrator.ProcessedTypes.Should().BeEquivalentTo(new[] { typeof(object) });
		}

		[Fact]
		public void should_match_all_filters()
		{
			var registrator = new ConventionalRegistrator();
			var container = new Container();

			container.Configure(x => x.Scan
				.Types(new[] { typeof(object), typeof(int),typeof(string) })
				.Matching(t => t.IsClass)
				.Matching(t => t.Name.Contains("n"))
				.Using(registrator));

			registrator.ProcessedTypes.Should().BeEquivalentTo(new[] { typeof(string) });
		}

		private class ConventionalRegistrator : IConventionalRegistrator
		{
			public ConventionalRegistrator()
			{
				ProcessedTypes = Enumerable.Empty<Type>();
			}

			public IEnumerable<Type> ProcessedTypes { get; private set; }

			public void Process(IEnumerable<Type> types, IContainerConfiguration containerConfiguration)
			{
				ProcessedTypes = types.ToList();
			}
		}

		public class ClassFilter : IConventionalRegistrationFilter
		{
			public bool IsMatch(Type type)
			{
				return type.IsClass;
			}
		}
	}
}