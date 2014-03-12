using FluentAssertions;
using Maestro.Configuration;
using Maestro.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Maestro.Tests
{
	public class default_settings
	{
		[Fact]
		public void lifetime_should_be_applied_to_all_instances()
		{
			var container = new Container(x =>
													{
														x.Default.Lifetime.Singleton();
														x.For<object>().Use<object>();
														x.For<IConditional>().Use(y =>
																										 {
																											 y.Else.Use<DefaultConditional>();
																											 y.If(ctx => ctx.Name != null).Use<NamedConditional>();
																										 });
													});

			var object1 = container.Get<object>();
			var object2 = container.Get<object>();
			object1.Should().Be(object2);

			var conditional1 = container.Get<IConditional>();
			var conditional2 = container.Get<IConditional>();
			conditional1.Should().Be(conditional2);

			var conditional3 = container.Get<IConditional>("xyz");
			var conditional4 = container.Get<IConditional>("xyz");
			conditional3.Should().Be(conditional4);
		}

		public interface IConditional { }
		public class DefaultConditional : IConditional { }
		public class NamedConditional : IConditional { }

		[Fact]
		public void configured_filter_should_be_used_in_conventional_registrations()
		{
			var types = new[] { typeof(object), typeof(int) };
			var convention = new Convention();
			new Container(x =>
							  {
								  x.Default.Filters.Add(t => !t.Name.Contains("n"));
								  x.Scan.Types(types).Using(convention);
							  });
			convention.Types.Should().BeEquivalentTo(new[] { typeof(object) });
		}

		public class Convention : IConvention
		{
			public IEnumerable<Type> Types { get; private set; }

			public void Process(IEnumerable<Type> types, IContainerExpression containerExpression)
			{
				Types = types.ToList();
			}
		}
	}
}