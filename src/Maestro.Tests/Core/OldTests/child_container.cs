using Shouldly;
using Xunit;

namespace Maestro.Tests.Core
{
	public class child_container
	{
		[Fact]
		public void should_fallback_to_use_root_config()
		{
			var expectedInstance = new object();
			var rootContainer = new Container(x => x.Use<object>().Instance(expectedInstance));
			var childContainer = rootContainer.GetChildContainer();

			var rootInstance = rootContainer.GetService<object>();
			var childInstance = childContainer.GetService<object>();

			rootInstance.ShouldBe(expectedInstance);
			rootInstance.ShouldBe(childInstance);
		}

		[Fact]
		public void child_config_should_not_be_used_for_instances_resolved_directly_from_root_container()
		{
			var rootContainer = new Container();
			var childContainer = rootContainer.GetChildContainer(x => x.Use<IDependency>().Type<Dependency>());

			var rootInstance = rootContainer.GetService<ClassWithOptionalDependency>();

			rootInstance.Dependency.ShouldBe(null);
		}

		[Fact]
		public void child_config_should_be_used_for_instances_resolved_directly_from_child_container()
		{
			var rootContainer = new Container();
			var childContainer = rootContainer.GetChildContainer(x => x.Use<IDependency>().Type<Dependency>());

			var childInstance = childContainer.GetService<ClassWithOptionalDependency>();

			childInstance.Dependency.ShouldNotBe(null);
		}

		[Fact]
		public void constructor_selection_should_be_reevaluated_if_parent_container_is_reconfigured()
		{
			var rootContainer = new Container();
			var childContainer = rootContainer.GetChildContainer();

			var instance1 = childContainer.GetService<ClassWithOptionalDependency>();
			rootContainer.Configure(x => x.Use<IDependency>().Type<Dependency>());
			var instance2 = childContainer.GetService<ClassWithOptionalDependency>();

			instance1.Dependency.ShouldBe(null);
			instance2.Dependency.ShouldNotBe(null);
		}

		interface IDependency { }
		class Dependency : IDependency { }

		private class ClassWithOptionalDependency
		{
			public ClassWithOptionalDependency() { }

			public ClassWithOptionalDependency(IDependency dependency)
			{
				Dependency = dependency;
			}

			public IDependency Dependency { get; }
		}
	}
}