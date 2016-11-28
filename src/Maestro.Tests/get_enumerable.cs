using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace Maestro.Tests
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
														x.Services<object>().Add.Instance(o1);
														x.Services<object>().Add.Instance(o2);
														x.Service<IEnumerable<object>>().Use.Instance(new[] { o3 });
													});

			var objects = container.Get<IEnumerable<object>>();

			objects.ShouldBe(new[] { o3 });
		}

		[Fact]
		public void get_enumerable_from_container_should_fall_back_to_get_all_if_explicit_instance_isnt_configured()
		{
			var o1 = new object();
			var o2 = new object();
			var container = new Container(x =>
													{
														x.Services<object>().Add.Instance(o1);
														x.Services<object>().Add.Instance(o2);
													});

			var objects = container.Get<IEnumerable<object>>();

			objects.ShouldBe(new[] { o1, o2 });
		}

		[Fact]
		public void get_all_from_container_should_not_use_explicit_instance_config()
		{
			var o1 = new object();
			var o2 = new object();
			var o3 = new object();
			var container = new Container(x =>
													{
														x.Services<object>().Add.Instance(o1);
														x.Services<object>().Add.Instance(o2);
														x.Service<IEnumerable<object>>().Use.Instance(new[] { o3 });
													});

			var objects = container.GetAll<object>();

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
														x.Services<object>().Add.Instance(o1);
														x.Services<object>().Add.Instance(o2);
														x.Service<IEnumerable<object>>().Use.Instance(new[] { o3 });
														x.Service<Instance>().Use.Type<Instance>().SetProperty(y => y.Objects, ctx => ctx.Get<IEnumerable<object>>());
													});

			var instance = container.Get<Instance>();

			instance.Objects.ShouldBe(new[] { o3 });
		}

		[Fact]
		public void get_enumerable_from_context_should_fall_back_to_get_all_if_explicit_instance_isnt_configured()
		{
			var o1 = new object();
			var o2 = new object();
			var container = new Container(x =>
													{
														x.Services<object>().Add.Instance(o1);
														x.Services<object>().Add.Instance(o2);
														x.Service<Instance>().Use.Type<Instance>().SetProperty(y => y.Objects, ctx => ctx.Get<IEnumerable<object>>());
													});

			var instance = container.Get<Instance>();

			instance.Objects.ShouldBe(new[] { o1, o2 });
		}

		[Fact]
		public void get_all_from_context_should_not_use_explicit_instance_config()
		{
			var o1 = new object();
			var o2 = new object();
			var o3 = new object();
			var container = new Container(x =>
													{
														x.Services<object>().Add.Instance(o1);
														x.Services<object>().Add.Instance(o2);
														x.Service<IEnumerable<object>>().Use.Instance(new[] { o3 });
														x.Service<Instance>().Use.Type<Instance>().SetProperty(y => y.Objects, ctx => ctx.GetAll<object>());
													});

			var instance = container.Get<Instance>();

			instance.Objects.ShouldBe(new[] { o1, o2 });
		}

		class Instance
		{
			public IEnumerable<object> Objects { get; set; }
		}
	}
}

