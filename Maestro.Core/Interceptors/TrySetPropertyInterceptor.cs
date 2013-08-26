namespace Maestro.Interceptors
{
	public class TrySetPropertyInterceptor : InterceptorBase
	{
		private readonly string _property;

		public TrySetPropertyInterceptor(string property)
		{
			_property = property;
		}

		public override IInterceptor Clone()
		{
			return new TrySetPropertyInterceptor(_property);
		}

		public override object Execute(object instance, IContext context)
		{
			var property = instance.GetType().GetProperty(_property);
			var type = property.PropertyType;
			if (context.CanGet(type))
			{
				var value = context.Get(type);
				property.SetValue(instance, value, null);
			}
			return instance;
		}
	}
}