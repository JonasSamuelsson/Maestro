using System;
using System.Linq;
using Maestro.Configuration;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Interception
{
	public class property_injection_tests
	{
		[Fact]
		public void set_property_using_auto_injection()
		{
			var continer = new Container(x =>
			{
				x.Use<string>().Instance("success-1");
				x.Use<Wrapper<string>>().Self().SetProperty(y => y.Value);
				x.Use<string>("x").Instance("success-2");
				x.Use<Wrapper<string>>("x").Self().SetProperty(y => y.Value);
				x.Use<string>("y").Instance("success-3");
				x.Use(typeof(Wrapper<>), "y").Self().SetProperty("Value");
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
				x.Add<Wrapper<string>>().Self().SetProperty("Value", "success");
				x.Add<Wrapper<string>>().Self().SetProperty(y => y.Value, "success");
			});

			container.GetServices<Wrapper<string>>().ForEach(x => x.Value.ShouldBe("success"));
		}

		[Fact]
		public void set_property_using_provided_factory()
		{
			var container = new Container(x =>
			{
				x.Use<string>().Instance("success");
				x.Add<Wrapper<string>>().Self().SetProperty("Value", () => "success");
				x.Add<Wrapper<string>>().Self().SetProperty("Value", ctx => ctx.GetService<string>());
				x.Add<Wrapper<string>>().Self().SetProperty("Value", (ctx, type) => ctx.GetService(type));
				x.Use(typeof(Wrapper<>)).Self().SetProperty("Value", (ctx, type) => ctx.GetService(type));
				x.Add<Wrapper<string>>().Self().SetProperty(y => y.Value, () => "success");
				x.Add<Wrapper<string>>().Self().SetProperty(y => y.Value, ctx => ctx.GetService<string>());
			});

			container.GetServices<Wrapper<string>>().ForEach(x => x.Value.ShouldBe("success"));
		}

		[Fact]
		public void try_set_property()
		{
			var container = new Container(x =>
			{
				x.Use<string>().Instance("success");
				x.Add<Wrapper<string>>().Self().TrySetProperty("Value");
				x.Add<Wrapper<string>>().Self().TrySetProperty(y => y.Value);
				x.Add(typeof(Wrapper<>)).Self().TrySetProperty("Value");
			});

			container.GetServices<Wrapper<string>>().ForEach(x => x.Value.ShouldBe("success"));
		}

		[Fact]
		public void SetProperty_should_have_the_ability_to_ignore_properties_that_doesnt_exist()
		{
			var container = new Container(x =>
			{
				x.Add<Wrapper<string>>().Self().SetProperty("foobar", PropertyNotFoundAction.Ignore);
				x.Add<Wrapper<string>>().Self().SetProperty("foobar", 0, PropertyNotFoundAction.Ignore);
				x.Add<Wrapper<string>>().Self().SetProperty("foobar", () => throw new InvalidOperationException(), PropertyNotFoundAction.Ignore);
				x.Add<Wrapper<string>>().Self().SetProperty("foobar", ctx => throw new InvalidOperationException(), PropertyNotFoundAction.Ignore);
				x.Add<Wrapper<string>>().Self().SetProperty("foobar", (ctx, type) => throw new InvalidOperationException(), PropertyNotFoundAction.Ignore);
			});

			container.GetServices<Wrapper<string>>().Count().ShouldBe(5);
		}

		[Fact]
		public void TrySetProperty_should_have_the_ability_to_ignore_properties_that_doesnt_exist()
		{
			var container = new Container(x =>
			{
				x.Use<Wrapper<string>>().Self().TrySetProperty("foobar", PropertyNotFoundAction.Ignore);
			});

			container.GetService<Wrapper<string>>();
		}

		class Wrapper<T>
		{
			public T Value { get; set; }
		}
	}
}