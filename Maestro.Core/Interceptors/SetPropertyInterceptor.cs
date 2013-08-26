namespace Maestro.Interceptors
{
	public class SetPropertyInterceptor : InterceptorBase
	{
		private readonly string _property;

		public SetPropertyInterceptor(string property)
		{
			_property = property;
		}

		public override IInterceptor Clone()
		{
			return new SetPropertyInterceptor(_property);
		}

		public override object Execute(object instance, IContext context)
		{
			var property = instance.GetType().GetProperty(_property);
			var value = context.Get(property.PropertyType);
			property.SetValue(instance, value, null);
			return instance;
		}
	}
}