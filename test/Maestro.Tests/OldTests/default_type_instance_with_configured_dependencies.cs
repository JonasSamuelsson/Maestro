namespace Maestro.Tests.OldTests
{
	public class default_type_instance_with_configured_dependencies
	{
		[Todo]
		public void should_get_instance_with_single_ctor_dependency()
		{
			//var container = new RootScope(x =>
			//{
			//	x.For(typeof(TypeWithDefaultCtor)).Add(typeof(TypeWithDefaultCtor));
			//	x.For(typeof(TypeWithCtorDependency)).Add(typeof(TypeWithCtorDependency));
			//});

			//var o = container.Get<TypeWithCtorDependency>();

			//o.Should().NotBeNull();
			//o.Dependency.Should().NotBeNull();
		}

		private class TypeWithDefaultCtor { }

		private class TypeWithCtorDependency
		{
			public TypeWithCtorDependency(TypeWithDefaultCtor dependency)
			{
				Dependency = dependency;
			}

			public TypeWithDefaultCtor Dependency { get; set; }
		}
	}
}