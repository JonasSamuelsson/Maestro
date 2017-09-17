using Maestro.Configuration;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace Maestro.Tests.Core.Interception
{
	public class property_injection_tests
	{
		[Fact]
		public void set_property_using_auto_injection()
		{
			var continer = new Container(x =>
			{
				x.For<string>().Use.Instance("success-1");
				x.For<Wrapper<string>>().Use.Self().SetProperty(y => y.Value);
				x.For<string>("x").Use.Instance("success-2");
				x.For<Wrapper<string>>("x").Use.Self().SetProperty(y => y.Value);
				x.For<string>("y").Use.Instance("success-3");
				x.For(typeof(Wrapper<>), "y").Use.Self().SetProperty("Value");
			});

			continer.GetService<Wrapper<string>>().Value.ShouldBe("success-1");
			continer.GetService<Wrapper<string>>("x").Value.ShouldBe("success-2");
			continer.GetService<Wrapper<string>>("y").Value.ShouldBe("success-3");
		}

		[Fact]
		public void set_property_using_provided_value()
		{
			var container = new Container(x =>
			{
				x.For<Wrapper<string>>().Add.Self().SetProperty("Value", "success");
				x.For<Wrapper<string>>().Add.Self().SetProperty(y => y.Value, "success");
			});

			container.GetServices<Wrapper<string>>().ForEach(x => x.Value.ShouldBe("success"));
		}

		[Fact]
		public void set_property_using_provided_factory()
		{
			var container = new Container(x =>
			{
				x.For<string>().Use.Instance("success");
				x.For<Wrapper<string>>().Add.Self().SetProperty("Value", () => "success");
				x.For<Wrapper<string>>().Add.Self().SetProperty("Value", ctx => ctx.GetService<string>());
				x.For<Wrapper<string>>().Add.Self().SetProperty("Value", (ctx, type) => ctx.GetService(type));
				x.For(typeof(Wrapper<>)).Use.Self().SetProperty("Value", (ctx, type) => ctx.GetService(type));
				x.For<Wrapper<string>>().Add.Self().SetProperty(y => y.Value, () => "success");
				x.For<Wrapper<string>>().Add.Self().SetProperty(y => y.Value, ctx => ctx.GetService<string>());
			});

			container.GetServices<Wrapper<string>>().ForEach(x => x.Value.ShouldBe("success"));
		}

		[Fact]
		public void try_set_property()
		{
			var container = new Container(x =>
			{
				x.For<string>().Use.Instance("success");
				x.For<Wrapper<string>>().Add.Self().TrySetProperty("Value");
				x.For<Wrapper<string>>().Add.Self().TrySetProperty(y => y.Value);
				x.For(typeof(Wrapper<>)).Add.Self().TrySetProperty("Value");
			});

			container.GetServices<Wrapper<string>>().ForEach(x => x.Value.ShouldBe("success"));
		}

		[Fact]
		public void SetProperty_should_have_the_ability_to_ignore_properties_that_doesnt_exist()
		{
			var container = new Container(x =>
			{
				x.For<Wrapper<string>>().Add.Self().SetProperty("foobar", PropertyNotFoundAction.Ignore);
				x.For<Wrapper<string>>().Add.Self().SetProperty("foobar", 0, PropertyNotFoundAction.Ignore);
				x.For<Wrapper<string>>().Add.Self().SetProperty("foobar", () => throw new InvalidOperationException(), PropertyNotFoundAction.Ignore);
				x.For<Wrapper<string>>().Add.Self().SetProperty("foobar", ctx => throw new InvalidOperationException(), PropertyNotFoundAction.Ignore);
				x.For<Wrapper<string>>().Add.Self().SetProperty("foobar", (ctx, type) => throw new InvalidOperationException(), PropertyNotFoundAction.Ignore);
			});

			container.GetServices<Wrapper<string>>().Count().ShouldBe(5);
		}

		[Fact]
		public void TrySetProperty_should_have_the_ability_to_ignore_properties_that_doesnt_exist()
		{
			var container = new Container(x =>
			{
				x.For<Wrapper<string>>().Use.Self().TrySetProperty("foobar", PropertyNotFoundAction.Ignore);
			});

			container.GetService<Wrapper<string>>();
		}

		class Wrapper<T>
		{
			public T Value { get; set; }
		}
	}
}