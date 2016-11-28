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
																x.Service<IGrandParent>().Use.Type<GrandParent>();
																x.Service<IParent>().Use.Type<Parent>();
																x.Service<IChild>().Use.Type<Child>();
																x.Service<IGrandChild>().Use.Type<GrandChild>();
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