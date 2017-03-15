using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Configuration;
using Maestro.Conventions;
using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class default_settings
	{
		[Fact]
		public void configured_filter_should_be_used_in_conventional_registrations()
		{
			var types = new[] { typeof(object), typeof(int) };
			var convention = new Convention();
			new Container(x =>
							  {
								  x.Default.Filters.Add(t => !t.Name.Contains("n"));
								  x.Scan(_ => _.Types(types).With(convention));
							  });
			convention.Types.ShouldBe(new[] { typeof(object) });
		}

		[Fact]
		public void default_lifetime_should_be_transient()
		{
			var container = new Container(x => x.For<object>().Use.Self());

			var instance1 = container.GetService<object>();
			var instance2 = container.GetService<object>();

			instance1.ShouldNotBe(instance2);
		}

		[Fact]
		public void should_use_configured_default_lifetime()
		{
			var container = new Container(x =>
			{
				x.Default.Lifetime.Transient();
				x.For<object>("transient").Use.Self();

				x.Default.Lifetime.Singleton();
				x.For<object>("singleton").Use.Self();
			});

			var transient1 = container.GetService<object>("transient");
			var transient2 = container.GetService<object>("transient");

			transient1.ShouldNotBe(transient2);

			var singleton1 = container.GetService<object>("singleton");
			var singleton2 = container.GetService<object>("singleton");

			singleton1.ShouldBe(singleton2);
		}

		public class Convention : IConvention
		{
			public IEnumerable<Type> Types { get; private set; }

			public void Process(IEnumerable<Type> types, ContainerConfigurator containerConfigurator)
			{
				Types = types.ToList();
			}
		}
	}
}