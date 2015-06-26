using Shouldly;
using Xunit;

namespace Maestro.Tests
{
	public class error_messages
	{
		[Todo]
		public void get()
		{
			var container = new Container();

			Should.Throw<ActivationException>(() => container.Get<Unresolvable>());
		}

		[Todo]
		public void tryget()
		{
			var container = new Container(x => x.For<Unresolvable>().Use<Unresolvable>());

			Should.Throw<ActivationException>(() => container.Get<Unresolvable>());
		}

		[Todo]
		public void getall()
		{
			var container = new Container(x => x.For<Unresolvable>().Use<Unresolvable>());

			Should.Throw<ActivationException>(() => container.GetAll<Unresolvable>());
		}

		[Todo]
		public void canget_dependency() { }

		[Todo]
		public void get_dependency()
		{
			var container = new Container();

			Should.Throw<ActivationException>(() => container.Get<Resolvable>());
		}

		[Todo]
		public void tryget_dependency()
		{
			var container = new Container(x => x.For<Unresolvable>().Use<Unresolvable>());

			Resolvable resolvable;
			Should.Throw<ActivationException>(() => container.TryGet(out resolvable));
		}

		[Todo]
		public void getall_dependencies() { }

		[Todo]
		public void cyclic_dependency() { }

		class Resolvable { public Resolvable(Unresolvable unresolvable) { } }
		class Unresolvable { public Unresolvable(object o) { } }
	}
}