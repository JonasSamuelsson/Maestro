using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Configuration;
using Maestro.Conventions;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Core.Conventions
{
	public class conventional_registration
	{
		[Fact]
		public void should_use_provided_registrator()
		{
			var types = new[] { typeof(object) };
			var convention = new Convention();
			var container = new Container();

			container.Configure(x => x.Scan(_ => _.Types(types).With(convention)));

			convention.ProcessedTypes.ShouldBe(types);
		}

		[Fact]
		public void should_filter_types_using_provided_lambda()
		{
			var convention = new Convention();
			var container = new Container();

			container.Configure(x => x.Scan(_ => _.Types(new[] { typeof(object), typeof(int) }).Matching(t => t.IsClass).With(convention)));

			convention.ProcessedTypes.ShouldBe(new[] { typeof(object) });
		}

		[Fact]
		public void should_filter_types_using_provided_filter()
		{
			var convention = new Convention();
			var container = new Container();

			container.Configure(x => x.Scan(_ => _.Types(new[] { typeof(object), typeof(int) }).Matching(new IsClassFilter()).With(convention)));

			convention.ProcessedTypes.ShouldBe(new[] { typeof(object) });
		}

		[Fact]
		public void type_should_match_all_filters_to_get_processed()
		{
			var convention = new Convention();
			var container = new Container();

			container.Configure(x => x.Scan(_ => _.Types(new[] { typeof(object), typeof(int), typeof(string) }).Matching(t => t.IsClass).Matching(t => t.Name.Contains("n")).With(convention)));

			convention.ProcessedTypes.ShouldBe(new[] { typeof(string) });
		}

		private class Convention : IConvention
		{
			public Convention()
			{
				ProcessedTypes = Enumerable.Empty<Type>();
			}

			public IEnumerable<Type> ProcessedTypes { get; private set; }

			public void Process(IEnumerable<Type> types, IContainerExpression containerExpression)
			{
				ProcessedTypes = types.ToList();
			}
		}

		public class IsClassFilter : IFilter
		{
			public bool IsMatch(Type type)
			{
				return type.IsClass;
			}
		}
	}
}