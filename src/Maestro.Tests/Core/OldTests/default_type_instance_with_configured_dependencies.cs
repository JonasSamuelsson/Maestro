namespace Maestro.Tests.Core
{
	public class default_type_instance_with_configured_dependencies
	{
		[Todo]
		public void should_get_instance_with_single_ctor_dependency()
		{
			//var container = new Container(x =>
			//{
			//	x.For(typeof(TypeWithDefaultCtor)).Use(typeof(TypeWithDefaultCtor));
			//	x.For(typeof(TypeWithCtorDependency)).Use(typeof(TypeWithCtorDependency));
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