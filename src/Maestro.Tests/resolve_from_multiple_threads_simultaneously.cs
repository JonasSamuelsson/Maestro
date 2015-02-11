using System.Threading.Tasks;
using Castle.Core.Internal;
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
																x.For<IGrandParent>().Use<GrandParent>();
																x.For<IParent>().Use<Parent>();
																x.For<IChild>().Use<Child>();
																x.For<IGrandChild>().Use<GrandChild>();
															});

			var tasks = new[]
			            {
				            new Task(() => container.Get<IGrandParent>(), TaskCreationOptions.LongRunning),
				            new Task(() => container.Get<IGrandParent>(), TaskCreationOptions.LongRunning)
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
			public GrandParent(IParent parent, IChild child) { }
		}

		class Parent : IParent { }

		class Child : IChild
		{
			public Child(IGrandChild grandChild) { }
		}

		class GrandChild : IGrandChild { }
	}
}