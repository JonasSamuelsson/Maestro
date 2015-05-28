using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class child_container
	{
		[Fact]
		public void should_fallback_to_use_root_config()
		{
			var expectedInstance = new object();
			var rootContainer = new Container(x => x.For<object>().Use(expectedInstance));
			var childContainer = rootContainer.GetChildContainer();

			var rootInstance = rootContainer.Get<object>();
			var childInstance = childContainer.Get<object>();

			rootInstance.ShouldBe(expectedInstance);
			rootInstance.ShouldBe(childInstance);
		}

		[Fact]
		public void child_config_should_not_be_used_for_instances_resolved_directly_from_root_container()
		{
			var rootContainer = new Container();
			var childContainer = rootContainer.GetChildContainer(x => x.For<IDependency>().Use<Dependency>());

			var rootInstance = rootContainer.Get<ClassWithOptionalDependency>();

			rootInstance.Dependency.ShouldBe(null);
		}

		[Fact]
		public void child_config_should_be_used_for_instances_resolved_directly_from_child_container()
		{
			var rootContainer = new Container();
			var childContainer = rootContainer.GetChildContainer(x => x.For<IDependency>().Use<Dependency>());
			
			var childInstance = childContainer.Get<ClassWithOptionalDependency>();
			
			childInstance.Dependency.ShouldNotBe(null);
		}

		[Fact]
		public void GetAll_from_child_container_should_include_instances_from_parent_where_name_doesnt_match()
		{
			var rootInstance1 = new object();
			var rootInstance2 = new object();
			var rootInstance3 = new object();
			var rootInstance4 = new object();
			var childInstance1 = new object();
			var childInstance2 = new object();
			var childInstance3 = new object();
			var childInstance4 = new object();
			var rootContainer = new Container(x =>
														 {
															 x.For<object>().Use(rootInstance1);
															 x.For<object>("foo").Use(rootInstance2);
															 x.For<object>("root").Use(rootInstance3);
															 x.For<object>().Add(rootInstance4);
														 });
			var childContainer = rootContainer.GetChildContainer(x =>
																				  {
																					  x.For<object>().Use(childInstance1);
																					  x.For<object>("foo").Use(childInstance2);
																					  x.For<object>("child").Use(childInstance3);
																					  x.For<object>().Add(childInstance4);
																				  });

			var rootInstances = rootContainer.GetAll<object>();
			var childInstances = childContainer.GetAll<object>();

			rootInstances.ShouldBe(new[] { rootInstance1, rootInstance2, rootInstance3, rootInstance4 });
			childInstances.ShouldBe(new[] { childInstance1, childInstance2, childInstance3, childInstance4, rootInstance3, rootInstance4 });
		}

		[Fact]
		public void constructor_selection_should_be_reevaluated_if_parent_container_is_reconfigured()
		{
			var rootContainer = new Container();
			var childContainer = rootContainer.GetChildContainer();

			var instance1 = childContainer.Get<ClassWithOptionalDependency>();
			rootContainer.Configure(x => x.For<IDependency>().Use<Dependency>());
			var instance2 = childContainer.Get<ClassWithOptionalDependency>();

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