using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Core
{
	public class get_enumerable
	{
		[Fact]
		public void get_enumerable_from_container_should_use_explicit_instance_config()
		{
			var o1 = new object();
			var o2 = new object();
			var o3 = new object();
			var container = new Container(x =>
													{
														x.Add<object>().Instance(o1);
														x.Add<object>().Instance(o2);
														x.Use<IEnumerable<object>>().Instance(new[] { o3 });
													});

			var objects = container.GetService<IEnumerable<object>>();

			objects.ShouldBe(new[] { o3 });
		}

		[Fact]
		public void get_enumerable_from_container_should_fall_back_to_get_all_if_explicit_instance_isnt_configured()
		{
			var o1 = new object();
			var o2 = new object();
			var container = new Container(x =>
													{
														x.Add<object>().Instance(o1);
														x.Add<object>().Instance(o2);
													});

			var objects = container.GetService<IEnumerable<object>>();

			objects.ShouldBe(new[] { o1, o2 });
		}

		[Fact]
		public void get_enumerable_from_context_should_use_explicit_instance_config()
		{
			var o1 = new object();
			var o2 = new object();
			var o3 = new object();
			var container = new Container(x =>
													{
														x.Add<object>().Instance(o1);
														x.Add<object>().Instance(o2);
														x.Use<IEnumerable<object>>().Instance(new[] { o3 });
														x.Use<Instance>().Type<Instance>().SetProperty(y => y.Objects, ctx => ctx.GetService<IEnumerable<object>>());
													});

			var instance = container.GetService<Instance>();

			instance.Objects.ShouldBe(new[] { o3 });
		}

		[Fact]
		public void get_enumerable_from_context_should_fall_back_to_get_all_if_explicit_instance_isnt_configured()
		{
			var o1 = new object();
			var o2 = new object();
			var container = new Container(x =>
													{
														x.Add<object>().Instance(o1);
														x.Add<object>().Instance(o2);
														x.Use<Instance>().Type<Instance>().SetProperty(y => y.Objects, ctx => ctx.GetService<IEnumerable<object>>());
													});

			var instance = container.GetService<Instance>();

			instance.Objects.ShouldBe(new[] { o1, o2 });
		}

		class Instance
		{
			public IEnumerable<object> Objects { get; set; }
		}
	}
}

