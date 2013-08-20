using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Maestro.Tests
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

		[Fact]
		public void should_register_concrete_sub_classes_of_provided_type()
		{
			var types = GetType().GetNestedTypes(BindingFlags.NonPublic);
			var container = new Container(x => x.Scan.Types(types).AddConcreteSubClassesOf<IBaseType>());

			var instances = container.GetAll<IBaseType>().ToList();

			instances.Should().HaveCount(2);
			instances.Should().Contain(x => x.IsOfType<Implementation1>());
			instances.Should().Contain(x => x.IsOfType<Implementation2>());
		}

		private interface IBaseType { }
		private class Implementation1 : IBaseType { }
		private class Implementation2 : IBaseType { }

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

		public class IsClassFilter : IConventionalRegistrationFilter
		{
			public bool IsMatch(Type type)
			{
				return type.IsClass;
			}
		}
	}
}