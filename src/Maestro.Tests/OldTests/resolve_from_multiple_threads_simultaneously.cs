using System.Threading.Tasks;
using Xunit;

namespace Maestro.Tests
{
	public class resolve_from_multiple_threads_simultaneously
	{
		[Fact]
		public void test()
		{
			var container = new Container(x =>
															{
																x.For<IGrandParent>().Use.Type<GrandParent>();
																x.For<IParent>().Use.Type<Parent>();
																x.For<IChild>().Use.Type<Child>();
																x.For<IGrandChild>().Use.Type<GrandChild>();
															});

			var tasks = new[]
							{
								new Task(() => container.GetService<IGrandParent>(), TaskCreationOptions.LongRunning),
								new Task(() => container.GetService<IGrandParent>(), TaskCreationOptions.LongRunning),
								new Task(() => container.GetService<IGrandParent>(), TaskCreationOptions.LongRunning),
								new Task(() => container.GetService<IGrandParent>(), TaskCreationOptions.LongRunning),
								new Task(() => container.GetService<IGrandParent>(), TaskCreationOptions.LongRunning)
							};
			tasks.ForEach(x => x.Start());
			Task.WaitAll(tasks);
		}

		interface IGrandParent { }
		interface IParent { }
		interface IChild { }
		interface IGrandChild { }

		class GrandParent : IGrandParent
		{
			public GrandParent(IParent parent, IChild child, IGrandChild grandChild) { }
		}

		class Parent : IParent
		{
			public Parent(IChild child, IGrandChild grandChild) { }
		}

		class Child : IChild
		{
			public Child(IGrandChild grandChild) { }
		}

		class GrandChild : IGrandChild { }
	}
}