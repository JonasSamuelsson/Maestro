using System;
using System.Linq;
using System.Reflection;
using Maestro.FactoryProviders;

namespace Maestro.Internals
{
	internal class ConcreteClosedClassTypeFactoryResolver : ITypeFactoryResolver
	{
		public bool TryGet(Type type, Context context, out IPipeline pipeline)
		{
			pipeline = null;
			if (!type.IsConcreteClosedClass()) return false;
			var pipelines = from ctor in type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
								 let parameters = ctor.GetParameters()
								 orderby parameters.Length descending
								 where parameters.All(p => context.Kernel.CanGetDependency(p.ParameterType, context))
								 let factoryProvider = new TypeFactoryProvider(type) { Constructor = ctor }
								 select new Pipeline(new Plugin { FactoryProvider = factoryProvider });
			pipeline = pipelines.FirstOrDefault();
			return pipeline != null;
		}
	}
}