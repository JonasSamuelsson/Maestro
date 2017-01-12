using Shouldly;

namespace Maestro.Tests
{
	public class error_messages
	{
		[Todo]
		public void get()
		{
			var container = new Container(x =>
			{
				x.For<A>().Use.Self();
				x.For<B>().Use.Self();
			});

			Should.Throw<ActivationException>(() => container.GetService<A>());
		}

		[Todo]
		public void tryget()
		{
			var container = new Container(x =>
			{
				x.For<A>().Use.Self();
				x.For<B>().Use.Self();
			});

			Should.Throw<ActivationException>(() => container.GetService<A>());
		}

		[Todo]
		public void getall()
		{
			var container = new Container(x =>
			{
				x.For<A>().Use.Self();
				x.For<B>().Use.Self();
			});

			Should.Throw<ActivationException>(() => container.GetServices<A>());
		}

		[Todo]
		public void canget_dependency() { }

		[Todo]
		public void get_dependency()
		{
			var container = new Container(x =>
			{
				x.For<A>().Use.Factory(ctx => new A(ctx.GetService<B>()));
				x.For<B>().Use.Self();
			});

			Should.Throw<ActivationException>(() => container.GetService<A>()).Message.ShouldBeNull();
		}

		[Todo]
		public void tryget_dependency()
		{
			var container = new Container(x =>
			{
				x.For<A>().Use.Factory(ctx => new A(ctx.GetService<B>()));
				x.For<B>().Use.Self();
			});

			A instance;
			Should.Throw<ActivationException>(() => container.TryGetService(out instance));
		}

		[Todo]
		public void getall_dependencies() { }

		[Todo]
		public void cyclic_dependency()
		{
			var container = new Container();

			Should.Throw<ActivationException>(() => container.GetService<X>()).Message.ShouldBeNull();
		}

		class A { public A(B b) { } }
		class B { public B(object o) { } }
		class X { public X(X dependency) { } }
	}
}