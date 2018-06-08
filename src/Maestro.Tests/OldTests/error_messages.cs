using Shouldly;

namespace Maestro.Tests.OldTests
{
	public class error_messages
	{
		[Todo]
		public void get()
		{
			var container = new Container(x =>
			{
				x.Use<A>().Self();
				x.Use<B>().Self();
			});

			Should.Throw<ActivationException>(() => container.GetService<A>());
		}

		[Todo]
		public void tryget()
		{
			var container = new Container(x =>
			{
				x.Use<A>().Self();
				x.Use<B>().Self();
			});

			Should.Throw<ActivationException>(() => container.GetService<A>());
		}

		[Todo]
		public void getall()
		{
			var container = new Container(x =>
			{
				x.Use<A>().Self();
				x.Use<B>().Self();
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
				x.Use<A>().Factory(ctx => new A(ctx.GetService<B>()));
				x.Use<B>().Self();
			});

			Should.Throw<ActivationException>(() => container.GetService<A>()).Message.ShouldBeNull();
		}

		[Todo]
		public void tryget_dependency()
		{
			var container = new Container(x =>
			{
				x.Use<A>().Factory(ctx => new A(ctx.GetService<B>()));
				x.Use<B>().Self();
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