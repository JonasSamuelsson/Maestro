using Xunit;

namespace Maestro.Tests.Core
{
	public class TodoAttribute : FactAttribute
	{
		public TodoAttribute()
		{
			Skip = "todo";
		}
	}
}