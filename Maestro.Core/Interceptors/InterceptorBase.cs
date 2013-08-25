using System.Linq;

namespace Maestro.Interceptors
{
	public abstract class InterceptorBase : IInterceptor
	{
		public virtual IInterceptor Clone()
		{
			var defaultCtor = GetType().GetConstructors().FirstOrDefault(x => x.GetParameters().Length == 0);
			return defaultCtor == null
				? this
				: (IInterceptor)defaultCtor.Invoke(null);
		}

		public abstract object Execute(object instance, IContext context);
	}
}