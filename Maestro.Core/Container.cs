namespace Maestro
{
	public class Container : IContainer
	{
		private static IContainer _default;

		public static IContainer Default
		{
			get { return _default ?? (_default = new Container()); }
		}
	}
}