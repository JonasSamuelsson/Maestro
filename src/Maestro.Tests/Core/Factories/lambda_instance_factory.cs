using Shouldly;
using Xunit;

namespace Maestro.Tests.Core.Factories
{
	public class lambda_instance_factory
	{
		[Fact]
		public void should_delegate_instantiation_to_provided_lambda()
		{
			var o = new object();

			var container = new Container(x => x.For<object>().Use.Factory(() => o));
			var instance = container.GetService<object>();

			instance.ShouldBe(o);
		}

		[Fact]
		public void should_be_able_to_retrieve_dependencies()
		{
			var o = new object();
			var container = new Container(x =>
			{
				x.For<object>().Use.Instance(o);
				x.For<ClassWithDependency>().Use.Factory(ctx => new ClassWithDependency { Dependency = ctx.GetService<object>() });
			});

			var instance = container.GetService<ClassWithDependency>();

			instance.Dependency.ShouldBe(o);
		}

		class ClassWithDependency
		{
			public object Dependency { get; set; }
		}
	}
}