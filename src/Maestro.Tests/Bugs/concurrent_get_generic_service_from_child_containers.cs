using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Maestro.Tests.Bugs
{
	public class concurrent_get_generic_service_from_child_containers
	{
		[Fact]
		public void should_not_throw()
		{
			var container = new Container(x => x.Use(typeof(IList<>)).Type(typeof(List<>)));
			var child1 = container.GetScopedContainer();
			var child2 = container.GetScopedContainer();

			var tasks = new[]
			{
				new Task(() => child1.GetService<Top>(), TaskCreationOptions.LongRunning),
				new Task(() => child2.GetService<Top>(), TaskCreationOptions.LongRunning)
			};

			tasks.ForEach(x => x.Start());

			Task.WaitAll(tasks);
		}

		class Top
		{
			public Top(IList<int> list) { }
		}
	}
}