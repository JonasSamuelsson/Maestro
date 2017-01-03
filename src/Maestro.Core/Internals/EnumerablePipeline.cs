using System;
using System.Linq;
using System.Reflection;

namespace Maestro.Internals
{
	class EnumerablePipeline : IPipeline
	{
		private readonly MethodInfo _genericCastMethod;
		private readonly IPipeline _pipeline;

		public EnumerablePipeline(Type elementType, ServiceDescriptor serviceDescriptor)
		{
			var castMethod = typeof(Enumerable).GetMethod("Cast", BindingFlags.Public | BindingFlags.Static);
			_genericCastMethod = castMethod.MakeGenericMethod(elementType);
			_pipeline = new Pipeline(serviceDescriptor);
		}

		public object Execute(Context context)
		{
			var instances = _pipeline.Execute(context);
			return _genericCastMethod.Invoke(null, new object[] { instances });
		}
	}
}